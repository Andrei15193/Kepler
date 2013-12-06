using System;
using System.Collections.Generic;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class WhenStatementNode
        : StatementNode
    {
        internal WhenStatementNode(ParsedNode<Lexicon> whenParsedNode)
        {
            if (whenParsedNode != null)
                if (whenParsedNode.Name == KeplerRuleSet.WhenStatement)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> bodyParsedNodes = whenParsedNode[KeplerRuleSet.Body];

                    _condition = new ExpressionNode(whenParsedNode[KeplerRuleSet.BooleanExpression, 0]);
                    _then = new BodyNode(bodyParsedNodes[0]);
                    if (bodyParsedNodes.Count == 2)
                        _else = new BodyNode(bodyParsedNodes[1]);
                }
                else
                    throw new ArgumentException("Expected when node!", "whenParsedNode");
            else
                throw new ArgumentNullException("whenParsedNode");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.When;
            }
        }

        public ExpressionNode Condition
        {
            get
            {
                return _condition;
            }
        }

        public BodyNode Then
        {
            get
            {
                return _then;
            }
        }

        public BodyNode Else
        {
            get
            {
                return _else;
            }
        }

        private readonly ExpressionNode _condition;
        private readonly BodyNode _then;
        private readonly BodyNode _else;
    }
}
