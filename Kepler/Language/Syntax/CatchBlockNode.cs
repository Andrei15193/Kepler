using System;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class CatchBlockNode
    {
        internal CatchBlockNode(ParsedNode<Lexicon> catchBlockParsedNode)
        {
            if (catchBlockParsedNode != null)
                if (catchBlockParsedNode.Name == KeplerRuleSet.CatchBlockStatement)
                {
                    _exception = new VariableDeclarationNode(catchBlockParsedNode[KeplerRuleSet.VariableDeclaration, 0]);
                    _body = new BodyNode(catchBlockParsedNode[KeplerRuleSet.Body, 0]);
                }
                else
                    throw new ArgumentException("Expected catch block node!", "catchBlockParsedNode");
            else
                throw new ArgumentNullException("catchBlockParsedNode");
        }

        public VariableDeclarationNode Exception
        {
            get
            {
                return _exception;
            }
        }

        public BodyNode Body
        {
            get
            {
                return _body;
            }
        }

        private readonly VariableDeclarationNode _exception;
        private readonly BodyNode _body;
    }
}
