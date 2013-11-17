using System;
using System.Collections.Generic;
using System.Linq;

namespace Andrei15193.Kepler.AbstractCore
{
    public class RuleSet<TCode>
        where TCode : struct
    {
        public RuleSet(ILanguage<TCode> language, string defaultRule, bool ignoreRuleNameCase)
        {
            if (defaultRule != null)
                if (language != null)
                {
                    _defaultRule = defaultRule.Trim();
                    if (_defaultRule.Length > 0)
                    {
                        _language = language;
                        _ignoreRuleNameCase = ignoreRuleNameCase;
                        _defaultRule = defaultRule;
                        if (_ignoreRuleNameCase)
                            _rules = new SortedDictionary<string, IList<Rule<TCode>>>(StringComparer.CurrentCultureIgnoreCase);
                        else
                            _rules = new SortedDictionary<string, IList<Rule<TCode>>>(StringComparer.CurrentCulture);
                    }
                    else
                        throw new ArgumentException("Cannot be empty!", "defaultRule");
                }
                else
                    throw new ArgumentNullException("language");
            else
                throw new ArgumentNullException("defaultRule");
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

        public ParsedNode<TCode> Parse(IReadOnlyList<ScannedAtom<TCode>> atoms, string ruleName = null, int startIndex = 0, Rule<TCode> callingRule = null)
        {
            if (atoms != null)
                if (0 <= startIndex && startIndex < atoms.Count)
                {
                    _SetInfiniteRecurentRules();
                    IList<ParsedNode<TCode>> parsedNodes = new List<ParsedNode<TCode>>();

                    foreach (Rule<TCode> rule in _rules[(ruleName ?? _defaultRule)])
                        if (callingRule == null || !_infiniteRecurentRules[callingRule.Name].Contains(rule))
                        {
                            ParsedNode<TCode> parsedNode = rule.Parse(atoms, startIndex);

                            if (parsedNode != null)
                                parsedNodes.Add(parsedNode);
                        }

                    return parsedNodes.OrderByDescending(parsedNode => parsedNode.Atoms.Count).FirstOrDefault();
                }
                else
                    throw new ArgumentOutOfRangeException("startIndex");
            else
                throw new ArgumentNullException("atoms");
        }

        public string DefaultRule
        {
            get
            {
                return _defaultRule;
            }
        }

        public IReadOnlyCollection<Rule<TCode>> Rules
        {
            get
            {
                return _rules.SelectMany(rule => rule.Value).ToList();
            }
        }

        public ILanguage<TCode> Language
        {
            get
            {
                return _language;
            }
        }

        public bool IgnoreRuleNameCase
        {
            get
            {
                return _ignoreRuleNameCase;
            }
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

        private IEnumerable<Rule<TCode>> _GetInfiniteRecurentRules(string ruleName)
        {
            HashSet<Rule<TCode>> infiniteRecurenceRules = new HashSet<Rule<TCode>>();

            foreach (Rule<TCode> callableRule in _rules[ruleName].Where(rule => rule.RuleNodes[0].NodeType == RuleNodeType.Rule))
            {
                SortedSet<string> ruleGroupsVisited = new SortedSet<string>(IgnoreRuleNameCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture) { ruleName };
                Queue<Rule<TCode>> rulesToCheck = new Queue<Rule<TCode>>();
                rulesToCheck.Enqueue(callableRule);

                while (rulesToCheck.Count > 0)
                {
                    Rule<TCode> ruleToCheck = rulesToCheck.Dequeue();

                    foreach (Rule<TCode> callableRuleFromRuleToCheck in _rules[ruleToCheck.RuleNodes[0].RuleName].Where(rule => rule.RuleNodes[0].NodeType == RuleNodeType.Rule))
                        if (!ruleGroupsVisited.Add(callableRuleFromRuleToCheck.Name))
                        {
                            if (string.Compare(ruleName, callableRuleFromRuleToCheck.Name, IgnoreRuleNameCase) == 0)
                                infiniteRecurenceRules.Add(callableRule);
                        }
                        else
                            rulesToCheck.Enqueue(callableRuleFromRuleToCheck);
                }
            }

            return infiniteRecurenceRules;
        }

        private IDictionary<string, IEnumerable<Rule<TCode>>> _infiniteRecurentRules = null;
        private readonly bool _ignoreRuleNameCase;
        private readonly string _defaultRule;
        private readonly ILanguage<TCode> _language;
        private readonly IDictionary<string, IList<Rule<TCode>>> _rules;
    }
}
