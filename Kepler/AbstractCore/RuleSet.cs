using System;
using System.Collections.Generic;
using System.Linq;

namespace Andrei15193.Kepler.AbstractCore
{
	public class RuleSet<TCode>
		where TCode : struct
	{
		protected RuleSet(ILanguage<TCode> language, bool ignoreRuleNameCase)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			_language = language;
			if (_ignoreRuleNameCase = ignoreRuleNameCase)
				_rules = new SortedDictionary<string, IList<Rule<TCode>>>(StringComparer.CurrentCultureIgnoreCase);
			else
				_rules = new SortedDictionary<string, IList<Rule<TCode>>>(StringComparer.CurrentCulture);
		}

		public ILanguage<TCode> Language
		{
			get
			{
				return _language;
			}
		}
		public IReadOnlyCollection<Rule<TCode>> Rules
		{
			get
			{
				return _rules.SelectMany(rule => rule.Value).ToList();
			}
		}

		public bool IsRuleNameCaseIgnored
		{
			get
			{
				return _ignoreRuleNameCase;
			}
		}

		public void Add(string ruleName, IReadOnlyList<RuleNode<TCode>> ruleNodes)
		{
			if (ruleName != null)
				if (ruleNodes != null)
				{
					IList<Rule<TCode>> rulesWithSameName;

					if (_rules.TryGetValue(ruleName, out rulesWithSameName))
						rulesWithSameName.Add(new Rule<TCode>(ruleName, this, ruleNodes));
					else
						_rules.Add(ruleName, new List<Rule<TCode>> { new Rule<TCode>(ruleName, this, ruleNodes) });
					_infiniteRecurentRules = null;
				}
				else
					throw new ArgumentNullException("ruleNodes");
			else
				throw new ArgumentNullException("ruleName");
		}
		public void Add(string ruleName, params RuleNode<TCode>[] ruleNodes)
		{
			Add(ruleName, (IReadOnlyList<RuleNode<TCode>>)ruleNodes);
		}
		internal protected ParsedNode<TCode> Parse(IReadOnlyList<ScannedAtom<TCode>> atoms, string ruleName = null, int startIndex = 0, Rule<TCode> callingRule = null)
		{
			if (atoms == null)
				throw new ArgumentNullException("atoms");
			if (startIndex < 0 && atoms.Count <= startIndex)
				throw new ArgumentOutOfRangeException("startIndex");
			IList<RuleParseResult<TCode>> parsedRuleNodes = new List<RuleParseResult<TCode>>();

			_SetInfiniteRecurentRules();
			foreach (Rule<TCode> rule in _rules[ruleName])
				if (callingRule == null || !_infiniteRecurentRules[callingRule.Name].Contains(rule))
				{
					RuleParseResult<TCode> parsedRuleNode = rule.Parse(atoms, startIndex);

					if (parsedRuleNode.ParsedNode != null || parsedRuleNode.ErrorMessage != null)
						parsedRuleNodes.Add(parsedRuleNode);
				}

			IReadOnlyList<RuleParseResult<TCode>> orderedRuleParseResults = parsedRuleNodes.OrderByDescending(parsedNode => parsedNode.ParsedNode == null ? 0 : parsedNode.ParsedNode.Atoms.Count).ToList();
			RuleParseResult<TCode> ruleParsedResult = orderedRuleParseResults.FirstOrDefault();

			if (ruleParsedResult.ParsedNode != null)
				return ruleParsedResult.ParsedNode;
			else
			{
				ruleParsedResult = orderedRuleParseResults.FirstOrDefault(ruleParseResult => ruleParseResult.ErrorMessage != null);
				//if (ruleParsedResult.ErrorMessage != null)
				//	throw new ArgumentException(string.Format("Expected " + (ruleName ?? _defaultRule) + " at line: {0}, column: {1}",
				//											  atoms[startIndex].Line,
				//											  atoms[startIndex].Column));
				//else
				return null;
			}
		}
		private IEnumerable<Rule<TCode>> _GetInfiniteRecurentRules(string ruleName)
		{
			HashSet<Rule<TCode>> infiniteRecurenceRules = new HashSet<Rule<TCode>>();

			foreach (Rule<TCode> callableRule in _rules[ruleName].Where(rule => rule.RuleNodes[0].NodeType == RuleNodeType.Rule))
			{
				SortedSet<string> ruleGroupsVisited = new SortedSet<string>(IsRuleNameCaseIgnored ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) { ruleName };
				Queue<Rule<TCode>> rulesToCheck = new Queue<Rule<TCode>>();
				rulesToCheck.Enqueue(callableRule);

				while (rulesToCheck.Count > 0)
				{
					Rule<TCode> ruleToCheck = rulesToCheck.Dequeue();

					foreach (Rule<TCode> callableRuleFromRuleToCheck in _rules[ruleToCheck.RuleNodes[0].RuleName].Where(rule => rule.RuleNodes[0].NodeType == RuleNodeType.Rule))
						if (!ruleGroupsVisited.Add(callableRuleFromRuleToCheck.Name))
						{
							if (string.Compare(ruleName, callableRuleFromRuleToCheck.Name, IsRuleNameCaseIgnored) == 0)
								infiniteRecurenceRules.Add(callableRule);
						}
						else
							rulesToCheck.Enqueue(callableRuleFromRuleToCheck);
				}
			}

			return infiniteRecurenceRules;
		}
		private void _SetInfiniteRecurentRules()
		{
			if (_infiniteRecurentRules == null)
			{
				_infiniteRecurentRules = new SortedDictionary<string, IEnumerable<Rule<TCode>>>();
				foreach (string ruleName in _rules.Keys)
					_infiniteRecurentRules.Add(ruleName, _GetInfiniteRecurentRules(ruleName));
			}
		}

		private IDictionary<string, IEnumerable<Rule<TCode>>> _infiniteRecurentRules = null;
		private readonly bool _ignoreRuleNameCase;
		private readonly ILanguage<TCode> _language;
		private readonly IDictionary<string, IList<Rule<TCode>>> _rules;
	}
}
