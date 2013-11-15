using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class Rule<TCode>
        where TCode : struct
    {
        internal Rule(string name, RuleSet<TCode> ruleSet, IReadOnlyList<RuleNode<TCode>> ruleNodes)
        {
            if (name != null)
                if (ruleNodes != null)
                    if (ruleNodes != null)
                    {
                        _name = name.Trim();
                        if (_name.Length > 0)
                            if (ruleNodes.Count(ruleNode => ruleNode == null) == 0)
                            {
                                _ruleNodes = new ReadOnlyCollection<RuleNode<TCode>>(ruleNodes.ToList());
                                _ruleSet = ruleSet;
                            }
                            else
                                throw new ArgumentException("Cannot contain null values!", "ruleNodes");
                        else
                            throw new ArgumentException("Cannot be empty!", "name");
                    }
                    else
                        throw new ArgumentNullException("ruleNodes");
                else
                    throw new ArgumentNullException("ruleSet");
            else
                throw new ArgumentNullException("name");
        }

        internal Rule(string name, RuleSet<TCode> ruleSet, params RuleNode<TCode>[] ruleNodes)
            : this(name, ruleSet, (IReadOnlyList<RuleNode<TCode>>)ruleNodes)
        {
        }

        public ParsedNode<TCode> Parse(IReadOnlyList<ScannedAtom<TCode>> atoms, int startIndex = 0)
        {
            if (atoms != null)
                if (0 <= startIndex && startIndex < atoms.Count
                    || startIndex == 0 && _ruleNodes.Count == 0)
                {
                    int currentIndex = startIndex;
                    ParsedNode<TCode> resultNode = new ParsedNode<TCode>(Name);

                    using (IEnumerator<RuleNode<TCode>> ruleNode = _ruleNodes.GetEnumerator())
                        while (currentIndex < atoms.Count && resultNode != null && ruleNode.MoveNext())
                            switch (ruleNode.Current.NodeType)
                            {
                                case RuleNodeType.Atom:
                                    if (ruleNode.Current.IsSequence)
                                        while (currentIndex < atoms.Count
                                               && ruleNode.Current.AtomCode.Equals(atoms[currentIndex].Code))
                                            resultNode.Add(atoms[currentIndex++]);
                                    else
                                        if (ruleNode.Current.AtomCode.Equals(atoms[currentIndex].Code))
                                            resultNode.Add(atoms[currentIndex++]);
                                        else
                                            resultNode = null;
                                    break;
                                case RuleNodeType.Rule:
                                    ParsedNode<TCode> childNode = _ruleSet.Parse(atoms, ruleNode.Current.RuleName, currentIndex);

                                    if (ruleNode.Current.IsSequence)
                                    {
                                        while (childNode != null)
                                        {
                                            foreach (ParsedNode<TCode> grandChildNodes in childNode.ChildNodes)
                                            {
                                                grandChildNodes.Parent = resultNode;
                                                resultNode.Add(grandChildNodes, appendAtoms: false);
                                            }
                                            resultNode.Add(childNode.Atoms);
                                            currentIndex += childNode.Atoms.Count;
                                            if (currentIndex < atoms.Count)
                                                childNode = _ruleSet.Parse(atoms, ruleNode.Current.RuleName, currentIndex);
                                            else
                                                childNode = null;
                                        }
                                    }
                                    else
                                    {
                                        if (childNode != null)
                                        {
                                            childNode.Parent = resultNode;
                                            resultNode.Add(childNode);
                                            currentIndex += childNode.Atoms.Count;
                                        }
                                        else
                                            resultNode = null;
                                    }
                                    break;
                            }

                    return resultNode;
                }
                else
                    throw new ArgumentOutOfRangeException("startIndex");
            else
                throw new ArgumentNullException("atoms");
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public IReadOnlyList<RuleNode<TCode>> RuleNodes
        {
            get
            {
                return _ruleNodes;
            }
        }

        private readonly string _name;
        private readonly RuleSet<TCode> _ruleSet;
        private readonly IReadOnlyList<RuleNode<TCode>> _ruleNodes;
    }
}
