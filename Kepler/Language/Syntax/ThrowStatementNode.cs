using System;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class ThrowStatementNode
        : StatementNode
    {
        internal ThrowStatementNode(ParsedNode<Lexicon> throwParsedNode)
        {
            if (throwParsedNode != null)
                if (throwParsedNode.Name == KeplerRuleSet.ThrowStatement)
                {
                    if (throwParsedNode.ChildNodeGroups.Count != 0)
                        if (throwParsedNode.ChildNodeGroups[0] == KeplerRuleSet.FunctionCall)
                            _functionCall = new FunctionCallStatementNode(throwParsedNode[KeplerRuleSet.FunctionCall, 0]);
                        else
                            _accessor = new QualifiedIdentifierNode(throwParsedNode[KeplerRuleSet.QualifiedIdentifier, 0]);
                }
                else
                    throw new ArgumentException("Expected throw statement node!", "throwParsedNode");
            else
                throw new ArgumentNullException("throwParsedNode");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.Throw;
            }
        }

        public FunctionCallStatementNode FunctionCall
        {
            get
            {
                return _functionCall;
            }
        }

        public QualifiedIdentifierNode Accessor
        {
            get
            {
                return _accessor;
            }
        }

        private readonly QualifiedIdentifierNode _accessor = null;
        private readonly FunctionCallStatementNode _functionCall = null;
    }
}
