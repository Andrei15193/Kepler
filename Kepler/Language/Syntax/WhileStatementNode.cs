using System;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class WhileStatementNode
        : StatementNode
    {
        internal WhileStatementNode(ParsedNode<Lexicon> whileParsedNode)
        {
            if (whileParsedNode != null)
                if (whileParsedNode.Name == KeplerRuleSet.WhileStatement)
                {
                    _condition = new ExpressionNode(whileParsedNode[KeplerRuleSet.BooleanExpression, 0]);
                    _body = new BodyNode(whileParsedNode[KeplerRuleSet.Body, 0]);
                }
                else
                    throw new ArgumentException("Expected while node!", "whileParsedNode");
            else
                throw new ArgumentNullException("whileParsedNode");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.While;
            }
        }

        public ExpressionNode Condition
        {
            get
            {
                return _condition;
            }
        }

        public BodyNode Body
        {
            get
            {
                return _body;
            }
        }

        private readonly ExpressionNode _condition;
        private readonly BodyNode _body;
    }
}
