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
        internal Rule(string name, RuleSet<TCode> ruleSet, IReadOnlyList<RuleNode> ruleNodes)
        {
            if (name != null)
                if (ruleNodes != null)
                    if (ruleNodes != null)
                    {
                        _name = name.Trim();
                        if (_name.Length > 0)
                            if (ruleNodes.Count(ruleNode => ruleNode == null) == 0)
                            {
                                _ruleNodes = new ReadOnlyCollection<RuleNode>(ruleNodes.ToList());
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

        internal Rule(string name, RuleSet<TCode> ruleSet, params RuleNode[] ruleNodes)
            : this(name, ruleSet, (IReadOnlyList<RuleNode>)ruleNodes)
        {
        }

        public ParseNode<TCode> Parse(IReadOnlyList<ScannedAtom<TCode>> atoms, int startIndex = 0)
        {
            if (atoms != null)
                if (0 <= startIndex && startIndex < atoms.Count
                    || startIndex == 0 && _ruleNodes.Count == 0)
                {
                    int currentIndex = startIndex;
                    ParseNode<TCode> resultNode = new ParseNode<TCode>(Name);

                    using (IEnumerator<RuleNode> ruleNode = _ruleNodes.GetEnumerator())
                        while (resultNode != null && ruleNode.MoveNext())
                            switch (ruleNode.Current.NodeType)
                            {
                                case RuleNodeType.Atom:
                                    if (ruleNode.Current.IsSequence)
                                        while (currentIndex < atoms.Count
                                               && _ruleSet.Language.GetCode(ruleNode.Current.Name).Equals(atoms[currentIndex].Code))
                                            resultNode.Add(atoms[currentIndex++]);
                                    else
                                        if (_ruleSet.Language.GetCode(ruleNode.Current.Name).Equals(atoms[currentIndex].Code))
                                            resultNode.Add(atoms[currentIndex++]);
                                        else
                                            resultNode = null;
                                    break;
                                case RuleNodeType.Rule:
                                    ParseNode<TCode> childNode = _ruleSet.Parse(atoms, ruleNode.Current.Name, currentIndex);

                                    if (ruleNode.Current.IsSequence)
                                        while (childNode != null)
                                        {
                                            childNode.Parent = resultNode;
                                            resultNode.Add(childNode);
                                            currentIndex += childNode.Atoms.Count;
                                            childNode = _ruleSet.Parse(atoms, ruleNode.Current.Name, currentIndex);
                                        }
                                    else
                                        if (childNode != null)
                                        {
                                            childNode.Parent = resultNode;
                                            resultNode.Add(childNode);
                                            currentIndex += childNode.Atoms.Count;
                                        }
                                        else
                                            resultNode = null;
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

        public IReadOnlyList<RuleNode> RuleNodes
        {
            get
            {
                return _ruleNodes;
            }
        }

        private readonly string _name;
        private readonly RuleSet<TCode> _ruleSet;
        private readonly IReadOnlyList<RuleNode> _ruleNodes;
    }
}
