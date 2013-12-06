using System;
using System.Collections.Generic;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class VariableAssignmentStatementNode
        : StatementNode
    {
        internal VariableAssignmentStatementNode(ParsedNode<Lexicon> variableAssignmentParsedNode)
        {
            if (variableAssignmentParsedNode != null)
                if (variableAssignmentParsedNode.Name == KeplerRuleSet.VariableAssignmentStatement)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childNodes;

                    _variableName = new QualifiedIdentifierNode(variableAssignmentParsedNode[KeplerRuleSet.QualifiedIdentifier, 0]);
                    if (variableAssignmentParsedNode.TryGetChildNodeGroup(KeplerRuleSet.Expression, out childNodes))
                        _value = new ExpressionNode(childNodes[0]);
                    else
                        _arrayInstance = new TypeInstanceNode(variableAssignmentParsedNode[KeplerRuleSet.TypeInstance, 0]);
                }
                else
                    throw new ArgumentException("Expected variable assignment statement node!", "variableAssignmentParsedNode");
            else
                throw new ArgumentNullException("variableAssignmentParsedNode");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.VariableAssignment;
            }
        }

        public QualifiedIdentifierNode VariableName
        {
            get
            {
                return _variableName;
            }
        }

        public ExpressionNode Value
        {
            get
            {
                return _value;
            }
        }

        public TypeInstanceNode ArrayInstance
        {
            get
            {
                return _arrayInstance;
            }
        }

        private readonly QualifiedIdentifierNode _variableName;
        private readonly ExpressionNode _value = null;
        private readonly TypeInstanceNode _arrayInstance = null;
    }
}
