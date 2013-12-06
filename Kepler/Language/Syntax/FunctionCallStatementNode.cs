using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class FunctionCallStatementNode
        : StatementNode
    {
        internal FunctionCallStatementNode(ParsedNode<Lexicon> functionCallParsedNode)
        {
            if (functionCallParsedNode != null)
                if (functionCallParsedNode.Name == KeplerRuleSet.FunctionCall)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> parameterExpressions;

                    _isConstructorCall = (functionCallParsedNode.Atoms[0].Code == Lexicon.New);
                    _functionName = new QualifiedIdentifierNode(functionCallParsedNode[KeplerRuleSet.QualifiedIdentifier, 0]);
                    if (functionCallParsedNode.TryGetChildNodeGroup(KeplerRuleSet.Expression, out parameterExpressions))
                        _parameters = parameterExpressions.Select(expression => new ExpressionNode(expression)).ToList();
                    else
                        _parameters = new ExpressionNode[0];
                }
                else
                    throw new ArgumentException("Expected function call parsed node", "functionCallParsedNode");
            else
                throw new ArgumentNullException("functionCallParsedNode");
        }

        public override string ToString()
        {
            if (_isConstructorCall)
                return ("new " + _functionName.ToString("::") + "(" + string.Join(", ", _parameters.Select(parameter => parameter.ToString())) + ")");
            else
                return (_functionName.ToString("::") + "(" + string.Join(", ", _parameters.Select(parameter => parameter.ToString())) + ")");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.FunctionCall;
            }
        }

        public QualifiedIdentifierNode FunctionName
        {
            get
            {
                return _functionName;
            }
        }

        public IReadOnlyList<ExpressionNode> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public bool IsConstructorCall
        {
            get
            {
                return _isConstructorCall;
            }
        }

        private readonly bool _isConstructorCall;
        private readonly QualifiedIdentifierNode _functionName;
        private readonly IReadOnlyList<ExpressionNode> _parameters;
    }
}
