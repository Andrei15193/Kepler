using System;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public abstract class OperandElement
        : ExpressionElement
    {
        public sealed class FunctionCall
            : OperandElement
        {
            internal FunctionCall(FunctionCallStatementNode functionCall)
            {
                if (functionCall != null)
                    _functionCall = functionCall;
                else
                    throw new ArgumentNullException("functionCall");
            }

            public override string ToString()
            {
                return (base.ToString() + " " + _functionCall.ToString());
            }

            public override OperandElementType OperandType
            {
                get
                {
                    return OperandElementType.FunctionCall;
                }
            }

            public FunctionCallStatementNode FunctionCallNode
            {
                get
                {
                    return _functionCall;
                }
            }

            private readonly FunctionCallStatementNode _functionCall;
        }

        public sealed class Accessor
            : OperandElement
        {
            internal Accessor(QualifiedIdentifierNode accessorPath)
            {
                if (accessorPath != null)
                    _accessorPath = accessorPath;
                else
                    throw new ArgumentNullException("accessorPath");
            }

            public override string ToString()
            {
                return (base.ToString() + " " + _accessorPath.ToString("::"));
            }

            public override OperandElementType OperandType
            {
                get
                {
                    return OperandElementType.Accessor;
                }
            }

            public QualifiedIdentifierNode AccessorPath
            {
                get
                {
                    return _accessorPath;
                }
            }

            private readonly QualifiedIdentifierNode _accessorPath;
        }

        public sealed class Constant
            : OperandElement
        {
            internal Constant(ParsedNode<Lexicon> constantParsedNode)
            {
                if (constantParsedNode != null)
                    if (constantParsedNode.Name == KeplerRuleSet.ArithmeticConstant
                        || constantParsedNode.Name == KeplerRuleSet.BooleanConstant)
                        switch (constantParsedNode.Atoms[0].Code)
                        {
                            case Lexicon.IntegerNumericConstant:
                                _constantNodeType = ConstantNodeType.Integer;
                                _value = constantParsedNode.Atoms[0].Value;
                                break;
                            case Lexicon.FloatNumericConstant:
                                _constantNodeType = ConstantNodeType.Float;
                                _value = constantParsedNode.Atoms[0].Value;
                                break;
                            case Lexicon.CharConstant:
                                _constantNodeType = ConstantNodeType.Character;
                                _value = constantParsedNode.Atoms[0].Value[1].ToString();
                                break;
                            case Lexicon.StringConstant:
                                _constantNodeType = ConstantNodeType.String;
                                _value = constantParsedNode.Atoms[0].Value.Substring(1, constantParsedNode.Atoms[0].Value.Length - 2);
                                break;
                            case Lexicon.True:
                                _constantNodeType = ConstantNodeType.Boolean;
                                _value = bool.TrueString;
                                break;
                            case Lexicon.False:
                                _constantNodeType = ConstantNodeType.Boolean;
                                _value = bool.FalseString;
                                break;
                            default:
                                throw new ArgumentException(constantParsedNode.Atoms[0].Code.ToString() + " is not a valid constant!", "constantParsedNode");
                        }
                    else
                        throw new ArgumentException("Must be constant node!", "constantParsedNode");
                else
                    throw new ArgumentNullException("constantParsedNode");
            }

            public override string ToString()
            {
                return (base.ToString() + " " + Value + " (" + ConstantType.ToString() + ")");
            }

            public override OperandElementType OperandType
            {
                get
                {
                    return OperandElementType.Constant;
                }
            }

            public ConstantNodeType ConstantType
            {
                get
                {
                    return _constantNodeType;
                }
            }

            public string Value
            {
                get
                {
                    return _value;
                }
            }

            private readonly string _value;
            private readonly ConstantNodeType _constantNodeType;
        }

        public override string ToString()
        {
            return (base.ToString() + " " + OperandType.ToString());
        }

        public abstract OperandElementType OperandType
        {
            get;
        }

        public sealed override ExpressionElementType ElementType
        {
            get
            {
                return ExpressionElementType.Operand;
            }
        }
    }
}
