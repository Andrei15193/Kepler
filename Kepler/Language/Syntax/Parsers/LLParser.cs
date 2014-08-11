using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Andrei15193.Kepler.Extensions;
using Andrei15193.Kepler.Extensions.Delegate;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax.Parsers
{
	public class LLParser<TAtomCode, TProductionRuleCode>
		: IParser<TAtomCode, TProductionRuleCode>
		where TAtomCode : struct
		where TProductionRuleCode : struct
	{
		public LLParser(int lookAheadCount = 1)
		{
			if (lookAheadCount < 0)
				throw new ArgumentException("Look ahead count cannot be negative!", "lookAheadCount");

			ObservableCollection<ProductionRule<TAtomCode, TProductionRuleCode>> productionRules = new ObservableCollection<ProductionRule<TAtomCode, TProductionRuleCode>>();

			productionRules.CollectionChanged += (sender, e) =>
				{
					switch (e.Action)
					{
						case NotifyCollectionChangedAction.Move:
							break;
						case NotifyCollectionChangedAction.Add:
						case NotifyCollectionChangedAction.Reset:
						case NotifyCollectionChangedAction.Replace:
							//foreach (ProductionRule<TAtomCode, TProductionRuleCode> duplicateProductionRule in productionRules.Intersect(e.NewItems.OfType<ProductionRule<TAtomCode, TProductionRuleCode>>()).ToList())
							//	productionRules.Remove(duplicateProductionRule);
							_parseTable = null;
							break;
						case NotifyCollectionChangedAction.Remove:
						default:
							_parseTable = null;
							break;
					}
				};
			_productionRules = productionRules;
			_lookAheadCount = lookAheadCount;
		}

		#region IParser Members
		public IParsedNode<TAtomCode, TProductionRuleCode> Parse(IReadOnlyList<ScannedAtom<TAtomCode>> atoms)
		{
			if (atoms == null)
				throw new ArgumentNullException("atoms");

			_Validate();
			_BuildParseTable();

			if (atoms.Count == 0)
				if (_productionRules.Any(productionRule => productionRule.Code.Equals(StartRuleCode) && productionRule.Symbols.Count == 0))
					return new ParsedNode<TAtomCode, TProductionRuleCode>(StartRuleCode);
				else
					throw new ArgumentException("Expecting statements at line 1, column 1", "atoms");

			int atomIndex = 0;
			Stack<ParsedNode<TAtomCode, TProductionRuleCode>> childParsedNodes = new Stack<ParsedNode<TAtomCode, TProductionRuleCode>>();
			Stack<EvaluationStackEntry> evaluationStack = new Stack<EvaluationStackEntry>();
			evaluationStack.Push(new EvaluationStackEntry(StartRuleCode));

			do
			{
				EvaluationStackEntry topEvaluationStackEntry = evaluationStack.Pop();
				if (topEvaluationStackEntry.Symbol.IsTerminal)
				{
					if (atomIndex == atoms.Count)
						throw new ArgumentException(string.Format("Expected statement at line {0}, column {1}", atoms[atoms.Count - 1].Line, atoms[atoms.Count - 1].Column), "atoms");
					childParsedNodes.Peek().Add(atoms[atomIndex++]);

					for (uint depth = (evaluationStack.Count > 0 ? evaluationStack.Peek().Depth : 1); depth < topEvaluationStackEntry.Depth; depth++)
					{
						ParsedNode<TAtomCode, TProductionRuleCode> topChildParsedNode = childParsedNodes.Pop();
						childParsedNodes.Peek().Add(topChildParsedNode, false);
					}
				}
				else
				{
					ProductionRule<TAtomCode, TProductionRuleCode> productionRule;
					IReadOnlyDictionary<IReadOnlyList<TAtomCode>, ProductionRule<TAtomCode, TProductionRuleCode>> parseTableRow = _parseTable[topEvaluationStackEntry.Symbol.NonTerminalCode];
					if (!parseTableRow.TryGetValue(_GetPrediction(atoms, atomIndex), out productionRule))
						throw new ArgumentException(string.Format("Expected {2} at line {0}, column {1}", atoms[atomIndex].Line, atoms[atomIndex].Column, topEvaluationStackEntry.Symbol.NonTerminalCode.ToString()), "atoms");

					foreach (ProductionRuleSymbol<TAtomCode, TProductionRuleCode> productionRuleSymbol in productionRule.Symbols.Reverse())
						evaluationStack.Push(new EvaluationStackEntry(productionRuleSymbol, topEvaluationStackEntry.Depth + 1));
					childParsedNodes.Push(new ParsedNode<TAtomCode, TProductionRuleCode>(productionRule.Code));
				}
			} while (evaluationStack.Count > 0);

			return childParsedNodes.Pop();
		}
		public TProductionRuleCode StartRuleCode
		{
			get;
			set;
		}
		public ICollection<ProductionRule<TAtomCode, TProductionRuleCode>> ProductionRules
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
			_ValidateLeftRecursion();
		}
		private void _ValidateProductionRules()
		{
			IReadOnlyCollection<TProductionRuleCode> unknownReferencedRules =
				_productionRules.SelectMany(productionRule => productionRule.Symbols
																			.Where(symbol => !symbol.IsTerminal && !_productionRules.Any(production => (production.Code.Equals(symbol.NonTerminalCode))))
																			.Select(symbol => symbol.NonTerminalCode))
																			.Distinct()
																			.ToList();
			if (unknownReferencedRules.Count > 0)
				throw new AggregateException("Thre are referenced rules that are not defined!",
											 unknownReferencedRules.Select(unknownReferencedRule => new InvalidOperationException(string.Format("Rule {0} is not defined!",
																																				unknownReferencedRule.ToString())))
																   .ToList());
		}
		private void _ValidateLeftRecursion()
		{
			Stack<Derivation> derivationTrace = new Stack<Derivation>();
			Stack<Derivation> derivations = new Stack<Derivation>(_productionRules.Where(productionRule => productionRule.Symbols.Count > 0
																										   && !productionRule.Symbols[0].IsTerminal)
																				  .Select(productionRule => new Derivation(productionRule)));

			while (derivations.Count > 0)
			{
				Derivation currentDerivation = derivations.Pop();

				if (derivationTrace.Count == 0 || derivationTrace.Peek().Depth < currentDerivation.Depth)
					derivationTrace.Push(currentDerivation);
				else
					do
						derivationTrace.Pop();
					while (derivationTrace.Count > 0 && derivationTrace.Peek().Depth >= currentDerivation.Depth);

				foreach (ProductionRule<TAtomCode, TProductionRuleCode> produtionRule in _productionRules.Where(productionRule => productionRule.Code.Equals(currentDerivation.ProductionRuleSymbols[0].NonTerminalCode)
																																  && productionRule.Symbols.Count > 0
																																  && !productionRule.Symbols[0].IsTerminal))
				{
					IReadOnlyList<ProductionRule<TAtomCode, TProductionRuleCode>> leftRecursiveRules =
						derivationTrace.SkipWhile(derivation => !derivation.ProductionRuleCode.Equals(produtionRule.Code))
									   .Cast<ProductionRule<TAtomCode, TProductionRuleCode>>()
									   .ToList();

					if (leftRecursiveRules.Count > 0)
						throw new InvalidOperationException(string.Join(Environment.NewLine, "These rules are causing left recursion:", string.Join(Environment.NewLine, leftRecursiveRules)));
					else
						derivations.Push(new Derivation(produtionRule, currentDerivation.Depth + 1));
				}
			}
		}
		private IEnumerable<IReadOnlyList<TAtomCode>> _First(IEnumerable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> productionRuleSymbols)
		{
			ISet<IReadOnlyList<TAtomCode>> firstSequencesSet = new HashSet<IReadOnlyList<TAtomCode>>(new DelegateEqualityComparer<IReadOnlyList<TAtomCode>>((left, right) => left.SequenceEqual(right),
																																							value => value.Aggregate(0, (hashCode, atomCode) => (hashCode ^ atomCode.GetHashCode()))));
			Queue<IReadOnlyList<TAtomCode>> terminalsSequences = new Queue<IReadOnlyList<TAtomCode>>();
			terminalsSequences.Enqueue(new TAtomCode[0]);

			foreach (ProductionRuleSymbol<TAtomCode, TProductionRuleCode> productionRuleSymbol in productionRuleSymbols)
				for (int terminalsSequencesCount = terminalsSequences.Count; terminalsSequencesCount > 0; terminalsSequencesCount--)
				{
					IReadOnlyList<TAtomCode> currentTerminalsSequence = terminalsSequences.Dequeue();

					foreach (IReadOnlyList<TAtomCode> terminals in _First(productionRuleSymbol))
					{
						IReadOnlyList<TAtomCode> terminalsSequence = currentTerminalsSequence.Concat(terminals)
																							 .Take(_lookAheadCount)
																							 .ToList();

						if (terminalsSequence.Count < _lookAheadCount)
							terminalsSequences.Enqueue(terminalsSequence);
						else
							firstSequencesSet.Add(terminalsSequence);
					}
				}

			while (terminalsSequences.Count > 0)
				firstSequencesSet.Add(terminalsSequences.Dequeue());
			return firstSequencesSet;
		}
		private IEnumerable<IReadOnlyList<TAtomCode>> _First(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> productionRuleSymbol)
		{
			if (productionRuleSymbol.IsTerminal)
				return new IReadOnlyList<TAtomCode>[] { new[] { productionRuleSymbol.TerminalCode } };

			IEnumerable<IReadOnlyList<TAtomCode>> cachedFirstSet;
			if (_first.TryGetValue(productionRuleSymbol.NonTerminalCode, out cachedFirstSet))
				return cachedFirstSet;

			ISet<IReadOnlyList<TAtomCode>> firstSet = new HashSet<IReadOnlyList<TAtomCode>>(new DelegateEqualityComparer<IReadOnlyList<TAtomCode>>((left, right) => left.SequenceEqual(right),
																																							value => value.Aggregate(0, (hashCode, atomCode) => (hashCode ^ atomCode.GetHashCode()))));
			var firstsToCalculate = (from productionRule in _productionRules
									 where productionRule.Code.Equals(productionRuleSymbol.NonTerminalCode)
									 select new
									 {
										 Symbols = productionRule.Symbols,
										 TerminalsCalculated = (IEnumerable<TAtomCode>)_emptyAtomCodeArray
									 }).ToQueue();

			do
			{

			} while (firstsToCalculate.Count > 0);

			//Queue<NonTerminalSymbolAtoms> productionRulesLeft = new Queue<NonTerminalSymbolAtoms>();
			//productionRulesLeft.Enqueue(new NonTerminalSymbolAtoms(productionRuleSymbol.NonTerminalCode, new TAtomCode[0]));

			//do
			//{
			//	NonTerminalSymbolAtoms productionRuleLeft = productionRulesLeft.Dequeue();

			//	foreach (ProductionRule<TAtomCode, TProductionRuleCode> productionRule in _productionRules.Where(productionRule => productionRule.Code.Equals(productionRuleLeft.ProductionRuleCode)))
			//		if (productionRule.Symbols.Count == 0)
			//			firstSet.Add(productionRuleLeft.Atoms);
			//		else
			//		{
			//			int symbolIndex = 0;
			//			List<TAtomCode> atoms = new List<TAtomCode>(productionRuleLeft.Atoms);

			//			while (symbolIndex < productionRule.Symbols.Count && productionRule.Symbols[symbolIndex].IsTerminal && atoms.Count < _lookAheadCount)
			//				atoms.Add(productionRule.Symbols[symbolIndex++].TerminalCode);

			//			if (symbolIndex == productionRule.Symbols.Count || atoms.Count == _lookAheadCount)
			//				firstSet.Add(atoms);
			//			else
			//				productionRulesLeft.Enqueue(new NonTerminalSymbolAtoms(productionRule.Symbols[symbolIndex].NonTerminalCode, atoms));
			//		}
			//} while (productionRulesLeft.Count > 0);

			_first.Add(productionRuleSymbol.NonTerminalCode, firstSet);
			return firstSet;
		}
		private IEnumerable<IReadOnlyList<TAtomCode>> _Follow(TProductionRuleCode productionRuleCode)
		{
			IEnumerable<IReadOnlyList<TAtomCode>> cachedFollowSet;
			if (_follow.TryGetValue(productionRuleCode, out cachedFollowSet))
				return cachedFollowSet;

			ISet<IReadOnlyList<TAtomCode>> followSet = new HashSet<IReadOnlyList<TAtomCode>>(_columnHeaderEqualityComparer);
			var followsToCalculate = new[]
				{
					new
					{
						ProductionRule = productionRuleCode,
						TerminalsCalculated = (IEnumerable<TAtomCode>)_emptyAtomCodeArray,
						VisitedProductionRulesWithoutFollow = (IEnumerable<TProductionRuleCode>)_emptyProductionRuleCodeArray
					}
				}.ToQueue();

			do
			{
				var followToCalculate = followsToCalculate.Dequeue();

				foreach (var follow in from productionRule in _productionRules
									   let followingSymbolsStartIndex = (productionRule.Symbols.IndexOf(followToCalculate.ProductionRule) + 1)
									   where (followingSymbolsStartIndex > 0)
									   select new
									   {
										   ProductionRule = productionRule.Code,
										   Symbols = _First(productionRule.Symbols.Skip(followingSymbolsStartIndex))
									   })
					foreach (IReadOnlyList<TProductionRuleCode> prediction in from followSymbols in follow.Symbols
																			  select followToCalculate.TerminalsCalculated
																									  .Concat(followSymbols)
																									  .Take(_lookAheadCount)
																									  .ToList())
						if (prediction.Count == 0)
							if (followToCalculate.VisitedProductionRulesWithoutFollow.Contains(follow.ProductionRule))
								followSet.Add(followToCalculate.TerminalsCalculated.ToList());
							else
								followsToCalculate.Enqueue(new
									{
										follow.ProductionRule,
										followToCalculate.TerminalsCalculated,
										VisitedProductionRulesWithoutFollow = followToCalculate.VisitedProductionRulesWithoutFollow
																							   .Append(followToCalculate.ProductionRule)
									});
						else
							if (prediction.Count == _lookAheadCount)
								followSet.Add(followToCalculate.TerminalsCalculated.ToList());
							else
								followsToCalculate.Enqueue(new
									{
										follow.ProductionRule,
										TerminalsCalculated = (IEnumerable<TAtomCode>)prediction,
										VisitedProductionRulesWithoutFollow = (IEnumerable<TProductionRuleCode>)_emptyProductionRuleCodeArray
									});
			} while (followsToCalculate.Count > 0);

			if (followSet.Count == 0)
				followSet.Add(_emptyAtomCodeArray);
			_follow.Add(productionRuleCode, followSet);

			return followSet;
		}
		private void _BuildParseTable()
		{
			if (_parseTable == null)
				_parseTable = _productionRules.GroupBy(productionRule => productionRule.Code)
											  .ToDictionary(productionRulesByCode => productionRulesByCode.Key,
															productionRulesByCode => _GetParseTableRow(productionRulesByCode));
		}
		private IReadOnlyDictionary<IReadOnlyList<TAtomCode>, ProductionRule<TAtomCode, TProductionRuleCode>> _GetParseTableRow(IEnumerable<ProductionRule<TAtomCode, TProductionRuleCode>> productionRules)
		{
			ISet<ProductionRule<TAtomCode, TProductionRuleCode>> conflictingRules = new HashSet<ProductionRule<TAtomCode, TProductionRuleCode>>();
			Dictionary<IReadOnlyList<TAtomCode>, ProductionRule<TAtomCode, TProductionRuleCode>> parseTableRow = new Dictionary<IReadOnlyList<TAtomCode>, ProductionRule<TAtomCode, TProductionRuleCode>>(_columnHeaderEqualityComparer);

			foreach (ProductionRule<TAtomCode, TProductionRuleCode> productionRule in productionRules)
				foreach (IReadOnlyList<TAtomCode> firstSequence in _First(productionRule.Symbols))
				{
					ProductionRule<TAtomCode, TProductionRuleCode> addedProductionRule;

					if (firstSequence.Count == _lookAheadCount)
					{
						if (!parseTableRow.TryGetValue(firstSequence, out addedProductionRule))
							parseTableRow.Add(firstSequence, productionRule);
						else
							if (addedProductionRule != productionRule)
							{
								conflictingRules.Add(productionRule);
								conflictingRules.Add(addedProductionRule);
							}
					}
					else
						foreach (IReadOnlyList<TAtomCode> prediction in _Follow(productionRule.Code).Select(followSequence => firstSequence.Concat(followSequence).Take(_lookAheadCount).ToList()))
							if (!parseTableRow.TryGetValue(prediction, out addedProductionRule))
								parseTableRow.Add(prediction, productionRule);
							else
								if (addedProductionRule != productionRule)
								{
									conflictingRules.Add(productionRule);
									conflictingRules.Add(addedProductionRule);
								}
				}

			if (conflictingRules.Count > 0)
				throw new ArgumentException(string.Join(Environment.NewLine,
														"These rules cause conflicts: ",
														string.Join(Environment.NewLine,
																	conflictingRules.Select(conflictingRule => conflictingRule.ToString()))));
			return parseTableRow;
		}
		private IReadOnlyList<TAtomCode> _GetPrediction(IReadOnlyList<ScannedAtom<TAtomCode>> atoms, int atomIndex)
		{
			TAtomCode[] prediction = new TAtomCode[Math.Min(atoms.Count - atomIndex, _lookAheadCount)];

			for (int predictionIndex = 0; predictionIndex < prediction.Length; predictionIndex++)
				prediction[predictionIndex] = atoms[atomIndex + predictionIndex].Code;

			return prediction;
		}

		private IReadOnlyDictionary<TProductionRuleCode, IReadOnlyDictionary<IReadOnlyList<TAtomCode>, ProductionRule<TAtomCode, TProductionRuleCode>>> _parseTable = null;
		private readonly int _lookAheadCount;
		private readonly ICollection<ProductionRule<TAtomCode, TProductionRuleCode>> _productionRules;
		private readonly IDictionary<TProductionRuleCode, IEnumerable<IReadOnlyList<TAtomCode>>> _first = new SortedDictionary<TProductionRuleCode, IEnumerable<IReadOnlyList<TAtomCode>>>();
		private readonly IDictionary<TProductionRuleCode, IEnumerable<IReadOnlyList<TAtomCode>>> _follow = new SortedDictionary<TProductionRuleCode, IEnumerable<IReadOnlyList<TAtomCode>>>();
		private static readonly TAtomCode[] _emptyAtomCodeArray = new TAtomCode[0];
		private static readonly TProductionRuleCode[] _emptyProductionRuleCodeArray = new TProductionRuleCode[0];
		private static readonly IEqualityComparer<IReadOnlyList<TAtomCode>> _columnHeaderEqualityComparer = new DelegateEqualityComparer<IReadOnlyList<TAtomCode>>((first, second) => first.SequenceEqual(second), value => value.Aggregate(0, (hashCode, terminal) => hashCode ^ terminal.GetHashCode()));

		private sealed class NonTerminalSymbolAtoms
		{
			public NonTerminalSymbolAtoms(TProductionRuleCode productionRuleCode, IReadOnlyList<TAtomCode> atoms)
			{
				_productionRuleCode = productionRuleCode;
				_atoms = atoms;
			}

			public override string ToString()
			{
				return string.Format("{0} -> {1}", _productionRuleCode.ToString(), string.Join(" ", _atoms));
			}
			public TProductionRuleCode ProductionRuleCode
			{
				get
				{
					return _productionRuleCode;
				}
			}
			public IReadOnlyList<TAtomCode> Atoms
			{
				get
				{
					return _atoms;
				}
			}

			private readonly TProductionRuleCode _productionRuleCode;
			private readonly IReadOnlyList<TAtomCode> _atoms;
		}
		private struct EvaluationStackEntry
			: IEquatable<EvaluationStackEntry>
		{
			internal EvaluationStackEntry(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> symbol, uint depth = 0)
			{
				_symbol = symbol;
				_depth = depth;
			}
			internal EvaluationStackEntry(TAtomCode terminal, uint depth = 0)
				: this(new ProductionRuleSymbol<TAtomCode, TProductionRuleCode>(terminal), depth)
			{
			}
			internal EvaluationStackEntry(TProductionRuleCode nonTerminal, uint depth = 0)
				: this(new ProductionRuleSymbol<TAtomCode, TProductionRuleCode>(nonTerminal), depth)
			{
			}

			public static explicit operator ProductionRuleSymbol<TAtomCode, TProductionRuleCode>(EvaluationStackEntry evaluationStackEntry)
			{
				return evaluationStackEntry._symbol;
			}
			public static bool operator ==(EvaluationStackEntry left, EvaluationStackEntry right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(EvaluationStackEntry left, object right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(EvaluationStackEntry left, ValueType right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(EvaluationStackEntry left, IEquatable<EvaluationStackEntry> right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(object left, EvaluationStackEntry right)
			{
				return right.Equals(left);
			}
			public static bool operator ==(ValueType left, EvaluationStackEntry right)
			{
				return right.Equals(left);
			}
			public static bool operator ==(IEquatable<EvaluationStackEntry> left, EvaluationStackEntry right)
			{
				return right.Equals(left);
			}
			public static bool operator !=(EvaluationStackEntry left, EvaluationStackEntry right)
			{
				return !(left == right);
			}
			public static bool operator !=(EvaluationStackEntry left, object right)
			{
				return !(left == right);
			}
			public static bool operator !=(EvaluationStackEntry left, ValueType right)
			{
				return !(left == right);
			}
			public static bool operator !=(EvaluationStackEntry left, IEquatable<EvaluationStackEntry> right)
			{
				return !(left == right);
			}
			public static bool operator !=(object left, EvaluationStackEntry right)
			{
				return !(left == right);
			}
			public static bool operator !=(ValueType left, EvaluationStackEntry right)
			{
				return !(left == right);
			}
			public static bool operator !=(IEquatable<EvaluationStackEntry> left, EvaluationStackEntry right)
			{
				return !(left == right);
			}

			#region IEquatable<EvaluationStackEntry> Members
			public bool Equals(EvaluationStackEntry other)
			{
				return (_symbol == other._symbol
						&& _depth == other._depth);
			}
			#endregion
			public override string ToString()
			{
				return string.Format("Symbol: {0}, depth: {1}", _symbol.ToString(), _depth);
			}
			public override bool Equals(object obj)
			{
				return (obj is EvaluationStackEntry && Equals((EvaluationStackEntry)obj));
			}
			public override int GetHashCode()
			{
				return (_symbol.GetHashCode() ^ _depth.GetHashCode());
			}

			internal ProductionRuleSymbol<TAtomCode, TProductionRuleCode> Symbol
			{
				get
				{
					return _symbol;
				}
			}
			internal uint Depth
			{
				get
				{
					return _depth;
				}
			}

			private readonly ProductionRuleSymbol<TAtomCode, TProductionRuleCode> _symbol;
			private readonly uint _depth;
		}
		private struct Derivation
			: IEquatable<Derivation>
		{
			internal Derivation(ProductionRule<TAtomCode, TProductionRuleCode> productionRule, uint depth = 0)
			{
				_productionRule = productionRule;
				_depth = depth;
			}

			public static explicit operator ProductionRule<TAtomCode, TProductionRuleCode>(Derivation derivation)
			{
				return derivation._productionRule;
			}
			public static bool operator ==(Derivation left, Derivation right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(Derivation left, object right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(Derivation left, ValueType right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(Derivation left, IEquatable<Derivation> right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(object left, Derivation right)
			{
				return right.Equals(left);
			}
			public static bool operator ==(ValueType left, Derivation right)
			{
				return right.Equals(left);
			}
			public static bool operator ==(IEquatable<Derivation> left, Derivation right)
			{
				return right.Equals(left);
			}
			public static bool operator !=(Derivation left, Derivation right)
			{
				return !(left == right);
			}
			public static bool operator !=(Derivation left, object right)
			{
				return !(left == right);
			}
			public static bool operator !=(Derivation left, ValueType right)
			{
				return !(left == right);
			}
			public static bool operator !=(Derivation left, IEquatable<Derivation> right)
			{
				return !(left == right);
			}
			public static bool operator !=(object left, Derivation right)
			{
				return !(left == right);
			}
			public static bool operator !=(ValueType left, Derivation right)
			{
				return !(left == right);
			}
			public static bool operator !=(IEquatable<Derivation> left, Derivation right)
			{
				return !(left == right);
			}

			#region IEquatable<Derivation> Members
			public bool Equals(Derivation other)
			{
				return (_depth == other._depth
						&& _productionRule == other._productionRule);
			}
			#endregion
			public override bool Equals(object obj)
			{
				return (obj is Derivation && Equals((Derivation)obj));
			}
			public override int GetHashCode()
			{
				return (_productionRule.GetHashCode() ^ _depth.GetHashCode());
			}
			public override string ToString()
			{
				return string.Format("(depth: {0}) {1}", _depth, _productionRule.ToString());
			}

			internal TProductionRuleCode ProductionRuleCode
			{
				get
				{
					return _productionRule.Code;
				}
			}
			internal IReadOnlyList<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> ProductionRuleSymbols
			{
				get
				{
					return _productionRule.Symbols;
				}
			}
			internal uint Depth
			{
				get
				{
					return _depth;
				}
			}

			private readonly ProductionRule<TAtomCode, TProductionRuleCode> _productionRule;
			private readonly uint _depth;
		}
		private struct ProductionRuleTerminals
			: IEquatable<ProductionRuleTerminals>
		{
			public ProductionRuleTerminals(TProductionRuleCode productionRuleCode, IReadOnlyList<TAtomCode> terminals = null, IEnumerable<TProductionRuleCode> visitedProductionRules = null)
			{
				_productionRuleCode = productionRuleCode;
				_terminals = (terminals ?? _emptyAtomCodeArray);
				_visitedProductionRules = (visitedProductionRules ?? visitedProductionRules);
			}

			public static bool operator ==(ProductionRuleTerminals left, ProductionRuleTerminals right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(ProductionRuleTerminals left, object right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(ProductionRuleTerminals left, ValueType right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(ProductionRuleTerminals left, IEquatable<ProductionRuleTerminals> right)
			{
				return left.Equals(right);
			}
			public static bool operator ==(object left, ProductionRuleTerminals right)
			{
				return right.Equals(left);
			}
			public static bool operator ==(ValueType left, ProductionRuleTerminals right)
			{
				return right.Equals(left);
			}
			public static bool operator ==(IEquatable<ProductionRuleTerminals> left, ProductionRuleTerminals right)
			{
				return right.Equals(left);
			}
			public static bool operator !=(ProductionRuleTerminals left, ProductionRuleTerminals right)
			{
				return !(left == right);
			}
			public static bool operator !=(ProductionRuleTerminals left, object right)
			{
				return !(left == right);
			}
			public static bool operator !=(ProductionRuleTerminals left, ValueType right)
			{
				return !(left == right);
			}
			public static bool operator !=(ProductionRuleTerminals left, IEquatable<ProductionRuleTerminals> right)
			{
				return !(left == right);
			}
			public static bool operator !=(object left, ProductionRuleTerminals right)
			{
				return !(left == right);
			}
			public static bool operator !=(ValueType left, ProductionRuleTerminals right)
			{
				return !(left == right);
			}
			public static bool operator !=(IEquatable<ProductionRuleTerminals> left, ProductionRuleTerminals right)
			{
				return !(left == right);
			}

			#region IEquatable<ProductionRuleTerminals> Members
			public bool Equals(ProductionRuleTerminals other)
			{
				return (_productionRuleCode.Equals(other._productionRuleCode)
						&& _terminals.SequenceEqual(other._terminals)
						&& _visitedProductionRules.SequenceEqual(other._visitedProductionRules));
			}
			#endregion
			public override bool Equals(object obj)
			{
				return (obj is ProductionRuleTerminals && Equals((ProductionRuleTerminals)obj));
			}
			public override int GetHashCode()
			{
				return (_productionRuleCode.GetHashCode()
						^ _terminals.Aggregate(0, (hashCode, terminal) => (hashCode ^ terminal.GetHashCode()))
						^ _visitedProductionRules.Aggregate(0, (hashCode, productionRuleCode) => (hashCode ^ productionRuleCode.GetHashCode())));
			}
			public override string ToString()
			{
				return string.Format("{0}, terminals: {{{1}}}, visited production rules: {{{2}}}",
									 _productionRuleCode.ToString(),
									 string.Join(", ", _terminals.Select(atomCode => atomCode.ToString())),
									 string.Join(", ", _visitedProductionRules.Select(productionRuleCode => productionRuleCode.ToString())));
			}
			public TProductionRuleCode ProductionRuleCode
			{
				get
				{
					return _productionRuleCode;
				}
			}
			public IReadOnlyList<TAtomCode> Terminals
			{
				get
				{
					return _terminals;
				}
			}
			public IEnumerable<TProductionRuleCode> VisitedProductionRules
			{
				get
				{
					return _visitedProductionRules;
				}
			}

			private readonly TProductionRuleCode _productionRuleCode;
			private readonly IReadOnlyList<TAtomCode> _terminals;
			private readonly IEnumerable<TProductionRuleCode> _visitedProductionRules;
		}
	}
}