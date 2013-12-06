using System;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class VariableDeclarationNode
    {
        internal VariableDeclarationNode(ParsedNode<Lexicon> variableDeclarationParsedNode)
        {
            if (variableDeclarationParsedNode != null)
                if (variableDeclarationParsedNode.Name == KeplerRuleSet.VariableDeclaration)
                {
                    _variableName = variableDeclarationParsedNode[KeplerRuleSet.Name, 0].Atoms[0].Value;
                    _type = new TypeNode(variableDeclarationParsedNode[KeplerRuleSet.Type, 0]);
                }
                else
                    throw new ArgumentException("Expected variable declaration", "variableDeclarationParsedNode");
            else
                throw new ArgumentNullException("variableDeclarationParsedNode");
        }

        public string VariableName
        {
            get
            {
                return _variableName;
            }
        }

        public TypeNode Type
        {
            get
            {
                return _type;
            }
        }

        private readonly string _variableName;
        private readonly TypeNode _type;
    }
}
