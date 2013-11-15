using System;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class RuleNode
    {
        static public RuleNode Atom(string name)
        {
            return new RuleNode(name, RuleNodeType.Atom, isSequence: false);
        }

        static public RuleNode AtomSequence(string name)
        {
            return new RuleNode(name, RuleNodeType.Atom, isSequence: true);
        }

        static public RuleNode Rule(string name)
        {
            return new RuleNode(name, RuleNodeType.Rule, isSequence: false);
        }

        static public RuleNode RuleSequence(string name)
        {
            return new RuleNode(name, RuleNodeType.Rule, isSequence: true);
        }

        public RuleNode(string name, RuleNodeType nodeType, bool isSequence = false)
        {
            if (name != null)
            {
                _name = name.Trim();
                if (_name.Length > 0)
                {
                    _nodeType = nodeType;
                    _isSequence = isSequence;
                }
                else
                    throw new ArgumentException("Cannot be empty!", "name");
            }
            else
                throw new ArgumentNullException("name");
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public RuleNodeType NodeType
        {
            get
            {
                return _nodeType;
            }
        }

        public bool IsSequence
        {
            get
            {
                return _isSequence;
            }
        }

        private readonly bool _isSequence;
        private readonly RuleNodeType _nodeType;
        private readonly string _name;
    }
}
