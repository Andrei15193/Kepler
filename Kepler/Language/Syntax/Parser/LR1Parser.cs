using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Andrei15193.Kepler.Extensions.Delegate;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public class LR1Parser
		: IParser
	{
		private class ParserConfigurtation
		{
			public ParserConfigurtation(IReadOnlyList<ScannedAtom> input)
			{
				if (input == null)
					throw new ArgumentNullException("input");
				_input = input;
			}

			public IReadOnlyList<ScannedAtom> Input
			{
				get
				{
					return _input;
				}
			}
			public IReadOnlyList<int> AppliedProductions
			{
				get
				{
					return _appliedProductions;
				}
			}

			private readonly IReadOnlyList<ScannedAtom> _input;
			private readonly List<int> _appliedProductions = new List<int>();
			private readonly Stack<ProductionRuleSymbol> _workStack = new Stack<ProductionRuleSymbol>();
		}
		private struct ParserState
			: IEquatable<ParserState>
		{
			private struct AugmentedProductionRule
				: IEquatable<AugmentedProductionRule>
			{
				public AugmentedProductionRule(ProductionRule productionRule, int dotLocation = 0, AtomCode? prediction = null)
				{
					if (dotLocation > productionRule.Symbols.Count)
						throw new ArgumentException("The dot cannot be placed farther than after the last symbol of the production!");
					_productionRule = productionRule;
					_dotLocation = dotLocation;
					_prediction = prediction;
				}

				public static explicit operator ProductionRule(AugmentedProductionRule augmentedProductionRule)
				{
					return augmentedProductionRule._productionRule;
				}

				#region IEquatable<AugmentedProductionRule> Members
				public bool Equals(AugmentedProductionRule other)
				{
					return (_productionRule.Equals(other._productionRule)
							&& _dotLocation == other._dotLocation
							&& _prediction.HasValue == other._prediction.HasValue
							&& _prediction.GetValueOrDefault() == other._prediction.GetValueOrDefault());
				}
				#endregion
				public ProductionRuleCode Code
				{
					get
					{
						return _productionRule.Code;
					}
				}
				public IReadOnlyList<ProductionRuleSymbol> Symbols
				{
					get
					{
						return _productionRule.Symbols;
					}
				}
				public int DotLocation
				{
					get
					{
						return _dotLocation;
					}
				}
				public AtomCode? Prediction
				{
					get
					{
						return _prediction;
					}
				}
				public override bool Equals(object obj)
				{
					return (obj is AugmentedProductionRule && Equals((AugmentedProductionRule)obj));
				}
				public override int GetHashCode()
				{
					return (_productionRule.GetHashCode() ^ _dotLocation.GetHashCode() ^ _prediction.GetValueOrDefault().GetHashCode());
				}
				public override string ToString()
				{
					StringBuilder stringRepresentationBuilder = new StringBuilder("[").Append(_productionRule.Code.ToString())
																		.Append(" ->");
					foreach (ProductionRuleSymbol productionRuleSymbolBeforeDot in _productionRule.Symbols.Take(_dotLocation))
						stringRepresentationBuilder.Append(' ').Append(productionRuleSymbolBeforeDot.ToString());
					stringRepresentationBuilder.Append(" .");
					foreach (ProductionRuleSymbol productionRuleSymbolAfterDot in _productionRule.Symbols.Skip(_dotLocation))
						stringRepresentationBuilder.Append(' ').Append(productionRuleSymbolAfterDot.ToString());
					stringRepresentationBuilder.Append(", ").Append(_prediction.ToString()).Append("]");
					return stringRepresentationBuilder.ToString();
				}
				public AugmentedProductionRule Goto(out ProductionRuleSymbol gotoSymbol)
				{
					gotoSymbol = _productionRule.Symbols[_dotLocation];
					return new AugmentedProductionRule(_productionRule, _dotLocation + 1, _prediction);
				}
				public bool CanGoto()
				{
					return (_dotLocation < _productionRule.Symbols.Count);
				}
				public bool CanClose(out ProductionRuleCode closingSymbol)
				{
					if (_dotLocation < _productionRule.Symbols.Count && !_productionRule.Symbols[_dotLocation].IsTerminal)
					{
						closingSymbol = _productionRule.Symbols[_dotLocation].NonTerminalCode;
						return true;
					}
					else
					{
						closingSymbol = 0;
						return false;
					}
				}
				public bool CanReduce()
				{
					return (_dotLocation == _productionRule.Symbols.Count);
				}

				private readonly int _dotLocation;
				private readonly ProductionRule _productionRule;
				private readonly AtomCode? _prediction;
			}

			private ParserState(IEnumerable<AugmentedProductionRule> augmentedProductionRules)
			{
				if (augmentedProductionRules == null)
					throw new ArgumentNullException("augmentedProductionRules");
				_augmentedProductionRules = augmentedProductionRules.Distinct().ToList();
				_gotos = new Dictionary<ProductionRuleSymbol, ParserState>();
			}
			public static ParserState Create(LR1Parser parser, ProductionRule startProductionRule)
			{
				ParserState startState = _Closure(new[] { new AugmentedProductionRule(startProductionRule, 0, null) }, parser);
				IDictionary<ParserState, ParserState> parserStates = new Dictionary<ParserState, ParserState>(_parserAugmentedRulesEqualityComparer);
				Queue<ParserState> statesLeftToBuild = new Queue<ParserState>();
				parserStates.Add(startState, startState);
				statesLeftToBuild.Enqueue(startState);
				do
				{
					ParserState currentState = statesLeftToBuild.Dequeue();
					foreach (IGrouping<ProductionRuleSymbol, AugmentedProductionRule> gotoResult in _ApplyGotos(currentState, parser))
					{
						ParserState existingState;
						ParserState gotoResultState = _Closure(gotoResult, parser);
						if (parserStates.TryGetValue(gotoResultState, out existingState))
							currentState._gotos.Add(gotoResult.Key, existingState);
						else
						{
							parserStates.Add(gotoResultState, gotoResultState);
							currentState._gotos.Add(gotoResult.Key, gotoResultState);
							statesLeftToBuild.Enqueue(gotoResultState);
						}
					}
				} while (statesLeftToBuild.Count > 0);
				_Validate(parserStates.Values);
				return startState;
			}

			#region IEquatable<ParserState> Members
			public bool Equals(ParserState other)
			{
				return (_augmentedProductionRules.Count == other._augmentedProductionRules.Count
						&& _gotos.Count == other._gotos.Count
						&& _augmentedProductionRules.All(other._augmentedProductionRules.Contains)
						&& other._augmentedProductionRules.All(_augmentedProductionRules.Contains)
						&& _gotos.All(other._gotos.Contains)
						&& other._gotos.All(_gotos.Contains));
			}
			#endregion
			public override bool Equals(object obj)
			{
				return (obj is ParserState && Equals((ParserState)obj));
			}
			public override int GetHashCode()
			{
				return _augmentedProductionRules.Aggregate(0, (hashCode, augmentedRule) => (hashCode ^ augmentedRule.GetHashCode()));
			}
			public override string ToString()
			{
				return ToString(Environment.NewLine);
			}
			public string ToString(string productionRuleSeparator)
			{
				return string.Join(productionRuleSeparator, _augmentedProductionRules.Select(productionRule => productionRule.ToString()));
			}

			private static ParserState _Closure(IEnumerable<AugmentedProductionRule> productionsToClose, LR1Parser parser)
			{
				bool hasChanges;
				ProductionRuleCode closingSymbol;
				HashSet<AugmentedProductionRule> parserStateProductionRules = new HashSet<AugmentedProductionRule>(productionsToClose);
				HashSet<AugmentedProductionRule> newProductions = new HashSet<AugmentedProductionRule>();
				do
				{
					foreach (AugmentedProductionRule parserStateProduction in parserStateProductionRules)
						if (parserStateProduction.CanClose(out closingSymbol))
							foreach (ProductionRule productionRule in parser.ProductionRules)
								if (closingSymbol == productionRule.Code)
									if (parserStateProduction.DotLocation + 1 < parserStateProduction.Symbols.Count)
									{
										IReadOnlyCollection<AtomCode> predictions = parser._First(parserStateProduction.Symbols[parserStateProduction.DotLocation + 1]);
										if (predictions.Count == 0)
											newProductions.Add(new AugmentedProductionRule(productionRule, 0, null));
										else
											foreach (AtomCode prediction in predictions)
												newProductions.Add(new AugmentedProductionRule(productionRule, 0, prediction));
									}
									else
										newProductions.Add(new AugmentedProductionRule(productionRule, 0, parserStateProduction.Prediction));
					hasChanges = false;
					foreach (AugmentedProductionRule newProduction in newProductions)
						if (parserStateProductionRules.Add(newProduction))
							hasChanges = true;
					newProductions.Clear();
				} while (hasChanges);
				return new ParserState(parserStateProductionRules);
			}
			private static IEnumerable<IGrouping<ProductionRuleSymbol, AugmentedProductionRule>> _ApplyGotos(ParserState state, LR1Parser parser)
			{
				ProductionRuleSymbol gotoSymbol;
				return from stateProductionRule in state._augmentedProductionRules
					   where stateProductionRule.CanGoto()
					   let gotoResult = new
					   {
						   Rule = stateProductionRule.Goto(out gotoSymbol),
						   Symbol = gotoSymbol
					   }
					   group gotoResult.Rule by gotoResult.Symbol;
			}
			private static void _Validate(IEnumerable<ParserState> parserStates)
			{
				ISet<IReadOnlyCollection<ProductionRule>> shiftReduceConflictedRulesGroups = new HashSet<IReadOnlyCollection<ProductionRule>>(_productionRuleSetEqualityComparer);
				ISet<IReadOnlyCollection<ProductionRule>> reduceReduceConflictedRulesGroups = new HashSet<IReadOnlyCollection<ProductionRule>>(_productionRuleSetEqualityComparer);

				foreach (ParserState parserState in parserStates)
				{
					IList<ProductionRule> reducibleRules = parserState._augmentedProductionRules
																	  .Where(productionRule => productionRule.CanReduce())
																	  .Select(productionRule => (ProductionRule)productionRule)
																	  .ToList();
					foreach (var reduceReduceConflictedRulesGroup in reducibleRules.GroupBy(reducibleRule => reducibleRule.Symbols, _symbolsSequenceEqualityComparer)
																				   .Select(grouping => grouping.Distinct().ToList())
																				   .Where(conflictCandidateRules => conflictCandidateRules.Count > 1))
						reduceReduceConflictedRulesGroups.Add(reduceReduceConflictedRulesGroup.ToList());
					foreach (AugmentedProductionRule shiftableProductionRule in parserState._augmentedProductionRules
																						   .Where(productionRule => !productionRule.CanReduce()))
					{
						IReadOnlyList<ProductionRuleSymbol> conflictablePart = shiftableProductionRule.Symbols
																									  .Take(shiftableProductionRule.DotLocation)
																									  .ToList();
						List<ProductionRule> conflictCandidateRules = reducibleRules.Where(productionRule => productionRule.Symbols.SequenceEqual(conflictablePart))
																					.Distinct()
																					.ToList();
						conflictCandidateRules.Add((ProductionRule)shiftableProductionRule);
						if (conflictCandidateRules.Count > 1)
							shiftReduceConflictedRulesGroups.Add(conflictCandidateRules);
					}
				}

				StringBuilder errorMessageBuilder = new StringBuilder();
				if (reduceReduceConflictedRulesGroups.Count > 0)
				{
					errorMessageBuilder.Append("Reduce reduce conflicts: ");
					foreach (IReadOnlyList<ProductionRule> conflictedProductionRulesGroup in reduceReduceConflictedRulesGroups)
					{
						errorMessageBuilder.Append(Environment.NewLine);
						foreach (ProductionRule conflictedProductionRule in conflictedProductionRulesGroup)
							errorMessageBuilder.Append(Environment.NewLine)
											   .Append(conflictedProductionRule.ToString());
					}
				}
				if (shiftReduceConflictedRulesGroups.Count > 0)
				{
					if (errorMessageBuilder.Length > 0)
						errorMessageBuilder.Append(Environment.NewLine)
										   .Append(Environment.NewLine)
										   .Append(Environment.NewLine);
					errorMessageBuilder.Append("Shift reduce conflicts: ");
					foreach (IReadOnlyList<ProductionRule> conflictedProductionRulesGroup in shiftReduceConflictedRulesGroups)
					{
						errorMessageBuilder.Append(Environment.NewLine);
						foreach (ProductionRule conflictedProductionRule in conflictedProductionRulesGroup)
							errorMessageBuilder.Append(Environment.NewLine)
											   .Append(conflictedProductionRule.ToString());
					}
				}
				if (errorMessageBuilder.Length > 0)
					throw new ArgumentException(errorMessageBuilder.ToString());
			}

			private readonly IReadOnlyCollection<AugmentedProductionRule> _augmentedProductionRules;
			private readonly IDictionary<ProductionRuleSymbol, ParserState> _gotos;
			private static readonly IEqualityComparer<ParserState> _parserAugmentedRulesEqualityComparer =
				new DelegateEqualityComparer<ParserState>((first, second) => (first._augmentedProductionRules.Count == second._augmentedProductionRules.Count
																			  && first._augmentedProductionRules.All(second._augmentedProductionRules.Contains)
																			  && second._augmentedProductionRules.All(first._augmentedProductionRules.Contains)),
														  parserState => parserState._augmentedProductionRules.Aggregate(0, (hashCode, augmentedRule) => (hashCode ^ augmentedRule.GetHashCode())));
			private static readonly IEqualityComparer<IReadOnlyList<ProductionRuleSymbol>> _symbolsSequenceEqualityComparer =
				new DelegateEqualityComparer<IReadOnlyList<ProductionRuleSymbol>>((first, second) => first.SequenceEqual(second),
																				  symbols => symbols.Aggregate(0, (hashCode, symbol) => (hashCode ^ symbol.GetHashCode())));
			private static readonly IEqualityComparer<IReadOnlyCollection<ProductionRule>> _productionRuleSetEqualityComparer =
				new DelegateEqualityComparer<IReadOnlyCollection<ProductionRule>>((first, second) => (first.Count == second.Count
																									  && first.All(second.Contains)
																									  && second.All(first.Contains)),
																				  symbols => symbols.Aggregate(0, (hashCode, symbol) => (hashCode ^ symbol.GetHashCode())));
		}

		#region IParser Members
		public ParsedNode Parse(IReadOnlyList<ScannedAtom> atoms)
		{
			_Validate();
			ParserState startState = ParserState.Create(this, new ProductionRule(0, new ProductionRuleSymbol(StartRule)));
			// do parsing here
			throw new NotImplementedException();
		}
		public ProductionRuleCode StartRule
		{
			get;
			set;
		}
		public IList<ProductionRule> ProductionRules
		{
			get
			{
				return _productionRules;
			}
		}
		#endregion

		private void _Validate()
		{
			if (_productionRules.Count == 0)
				throw new InvalidOperationException("There are no production rules! Cannot parse such language!");
			_ValidateProductionRules();
		}
		private void _ValidateProductionRules()
		{
			IReadOnlyCollection<ProductionRuleCode> unknownReferencedRules =
				_productionRules.SelectMany(productionRule => productionRule.Symbols
																			.Where(symbol => !symbol.IsTerminal && !_productionRules.Any(production => (production.Code == symbol.NonTerminalCode)))
																			.Select(symbol => symbol.NonTerminalCode))
								.Distinct()
								.ToList();
			if (unknownReferencedRules.Count > 0)
				throw new AggregateException("Thre are referenced rules that are not defined!",
											 unknownReferencedRules.Select(unknownReferencedRule => new IndexOutOfRangeException(string.Format("Rule {0} is not defined!",
																																			   unknownReferencedRule.ToString())))
																   .ToList());
		}
		private IReadOnlyCollection<AtomCode> _First(ProductionRuleSymbol symbol)
		{
			if (symbol.IsTerminal)
				return new[] { symbol.TerminalCode };
			return (from productionRule in _productionRules
					where (productionRule.Code == symbol.NonTerminalCode && productionRule.Symbols.Count > 0 && productionRule.Symbols[0].IsTerminal)
					select productionRule.Symbols[0].TerminalCode).ToList();
		}

		private readonly IList<ProductionRule> _productionRules = new List<ProductionRule>();
	}
}