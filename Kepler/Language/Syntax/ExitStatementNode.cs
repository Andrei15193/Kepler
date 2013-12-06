using System;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class ExitStatementNode
        : StatementNode
    {
        internal ExitStatementNode(ParsedNode<Lexicon> exitParsedNode)
        {
            if (exitParsedNode != null)
                if (exitParsedNode.Name == KeplerRuleSet.ExitStatement)
                    switch (exitParsedNode.Atoms[0].Code)
                    {
                        case Lexicon.Skip:
                            _isSkip = true;
                            break;
                        case Lexicon.Stop:
                            _isStop = true;
                            break;
                        default:
                            _assertion = new ExpressionNode(exitParsedNode[KeplerRuleSet.BooleanExpression, 0]);
                            break;
                    }
                else
                    throw new ArgumentException("Expected exist statement node!", "exitParsedNode");
            else
                throw new ArgumentNullException("exitParsedNode");
        }

        internal ExitStatementNode(ExpressionNode assertion)
        {
            if (assertion != null)
                _assertion = assertion;
            else
                throw new ArgumentNullException("assertion");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.Exit;
            }
        }

        public bool IsSkip
        {
            get
            {
                return _isSkip;
            }
        }

        public bool IsStop
        {
            get
            {
                return _isStop;
            }
        }

        public ExpressionNode Assertion
        {
            get
            {
                return _assertion;
            }
        }

        private readonly bool _isSkip = false;
        private readonly bool _isStop = false;
        private readonly ExpressionNode _assertion = null;
    }
}
