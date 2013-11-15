using System;
using System.Collections.Generic;
using System.Linq;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public class RuleSet<TCode>
        where TCode : struct
    {
        public RuleSet(ILanguage<TCode> language)
        {
            if (language != null)
                _language = language;
            else
                throw new ArgumentNullException("language");
        }

        public void Add(string ruleName, IReadOnlyList<RuleNode> ruleNodes)
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

        public void Add(string ruleName, params RuleNode[] ruleNodes)
        {
            Add(ruleName, (IReadOnlyList<RuleNode>)ruleNodes);
        }

        public ParseNode<TCode> Parse(IReadOnlyList<ScannedAtom<TCode>> atoms, string ruleName, int startIndex = 0)
        {
            if (atoms != null)
                if (ruleName != null)
                    if (0 <= startIndex && startIndex < atoms.Count)
                        return (from rule in _rules[ruleName]
                                let parseNode = rule.Parse(atoms, startIndex)
                                where parseNode != null
                                orderby parseNode.Atoms.Count descending
                                select parseNode).FirstOrDefault();
                    else
                        throw new ArgumentOutOfRangeException("startIndex");
                else
                    throw new ArgumentNullException("ruleName");
            else
                throw new ArgumentNullException("atoms");
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

        private readonly ILanguage<TCode> _language;
        private readonly IDictionary<string, IList<Rule<TCode>>> _rules = new SortedDictionary<string, IList<Rule<TCode>>>();
    }
}
