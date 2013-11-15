using System;
using System.Collections.Generic;
using System.Linq;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
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

        public ParsedNode<TCode> Parse(IReadOnlyList<ScannedAtom<TCode>> atoms, string ruleName = null, int startIndex = 0)
        {
            if (atoms != null)
                if (0 <= startIndex && startIndex < atoms.Count)
                    return (from rule in _rules[(ruleName ?? _defaultRule)]
                            let parseNode = rule.Parse(atoms, startIndex)
                            where parseNode != null
                            orderby parseNode.Atoms.Count descending
                            select parseNode).FirstOrDefault();
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

        private readonly bool _ignoreRuleNameCase;
        private readonly string _defaultRule;
        private readonly ILanguage<TCode> _language;
        private readonly IDictionary<string, IList<Rule<TCode>>> _rules;
    }
}
