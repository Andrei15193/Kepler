using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class VariableDeclarationStatementNode
        : StatementNode
    {
        internal VariableDeclarationStatementNode(ParsedNode<Lexicon> variableDeclarationParsedNode)
        {
            if (variableDeclarationParsedNode != null)
                if (variableDeclarationParsedNode.Name == KeplerRuleSet.VariableDeclarationStatement)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childNodes;

                    _variableDeclarationNode = new VariableDeclarationNode(variableDeclarationParsedNode[KeplerRuleSet.VariableDeclaration, 0]);
                    if (variableDeclarationParsedNode.TryGetChildNodeGroup(KeplerRuleSet.Expression, out childNodes))
                        _initialValue = new ExpressionNode(childNodes[0]);
                    else
                        _arrayInstance = new TypeInstanceNode(variableDeclarationParsedNode[KeplerRuleSet.TypeInstance, 0]);
                }
                else
                    throw new ArgumentException("Expected variable declaration statement node!", "variableDeclarationParsedNode");
            else
                throw new ArgumentNullException("variableDeclarationParsedNode");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.VariableDeclaration;
            }
        }

        public string VariableName
        {
            get
            {
                return _variableDeclarationNode.VariableName;
            }
        }

        public TypeNode VariableType
        {
            get
            {
                return _variableDeclarationNode.Type;
            }
        }

        public ExpressionNode InitialValue
        {
            get
            {
                return _initialValue;
            }
        }

        public TypeInstanceNode ArrayInstance
        {
            get
            {
                return _arrayInstance;
            }
        }

        private readonly VariableDeclarationNode _variableDeclarationNode;
        private readonly ExpressionNode _initialValue = null;
        private readonly TypeInstanceNode _arrayInstance = null;
    }
}
