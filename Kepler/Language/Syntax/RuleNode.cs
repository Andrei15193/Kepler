using System;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class RuleNode<TCode>
        where TCode : struct
    {
        static public RuleNode<TCode> Atom(TCode code)
        {
            return new RuleNode<TCode>(code, isSequence: false);
        }

        static public RuleNode<TCode> AtomSequence(TCode code)
        {
            return new RuleNode<TCode>(code, isSequence: true);
        }

        static public RuleNode<TCode> Rule(string name)
        {
            return new RuleNode<TCode>(name, isSequence: false);
        }

        static public RuleNode<TCode> RuleSequence(string name)
        {
            return new RuleNode<TCode>(name, isSequence: true);
        }

        private RuleNode(string ruleName, bool isSequence = false)
        {
            if (ruleName != null)
            {
                _ruleName = ruleName.Trim();
                if (_ruleName.Length > 0)
                {
                    _nodeType = RuleNodeType.Rule;
                    _isSequence = isSequence;
                    _atomCode = default(TCode);
                }
                else
                    throw new ArgumentException("Cannot be empty!", "name");
            }
            else
                throw new ArgumentNullException("name");
        }

        private RuleNode(TCode atomCode, bool isSequence = false)
        {
            _ruleName = null;
            _nodeType = RuleNodeType.Atom;
            _isSequence = isSequence;
            _atomCode = atomCode;
        }

        public TCode AtomCode
        {
            get
            {
                if (_nodeType == RuleNodeType.Atom)
                    return _atomCode;
                else
                    throw new InvalidOperationException("The RuleNode instance is on of type Atom");
            }
        }

        public string RuleName
        {
            get
            {
                if (_nodeType == RuleNodeType.Rule)
                    return _ruleName;
                else
                    throw new InvalidOperationException("The RuleNode instance is on of type Rule");
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
        private readonly TCode _atomCode;
        private readonly string _ruleName;
    }
}
