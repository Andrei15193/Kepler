using System;
using System.Collections.Generic;
using System.Linq;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public class PredicateDeclarationNode
    {
        static private ExpressionElement _CreateRelationOperator(ParsedNode<Lexicon> parsedNode)
        {
            if (parsedNode != null)
                if (parsedNode.Name == KeplerRuleSet.ArithmeticRelation)
                    switch (parsedNode.Atoms[0].Code)
                    {
                        case Lexicon.LessThan:
                            return new OperatorElement(OperationName.LessThanComparison);
                        case Lexicon.LessThanOrEqualTo:
                            return new OperatorElement(OperationName.LessThanOrEqualToComparison);
                        case Lexicon.Equal:
                            return new OperatorElement(OperationName.EqualComparison);
                        case Lexicon.GreaterThanOrEqualTo:
                            return new OperatorElement(OperationName.GreaterThanOrEqualToComparison);
                        case Lexicon.GreaterThan:
                            return new OperatorElement(OperationName.GreaterThanComparison);
                        default:
                            throw new ArgumentException("Expected arithmetic relation!", "parsedNode");
                    }
                else
                    throw new ArgumentException("Expected arithmetic relation!", "parsedNode");
            else
                throw new ArgumentNullException("parsedNode");
        }

        internal PredicateDeclarationNode(ParsedNode<Lexicon> predicateDefinition)
        {
            if (predicateDefinition != null)
                if (predicateDefinition.Name == KeplerRuleSet.PredicateDefinition)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> variableDeclarations;

                    _isFact = false;
                    _name = predicateDefinition[KeplerRuleSet.Name, 0].Atoms[0].Value;

                    if (predicateDefinition.TryGetChildNodeGroup(KeplerRuleSet.VariableDeclaration, out variableDeclarations))
                        _parameters = variableDeclarations.Select(variableDeclaration => new VariableDeclarationNode(variableDeclaration)).ToList();
                    else
                        _parameters = new List<VariableDeclarationNode>();
                    _body = new BodyNode(predicateDefinition[KeplerRuleSet.Body, 0]);
                }
                else
                    if (predicateDefinition.Name == KeplerRuleSet.FactDefinition)
                    {
                        IReadOnlyList<ParsedNode<Lexicon>> factParameters;
                        List<ExpressionElement> assertion = new List<ExpressionElement>();

                        _isFact = true;
                        _name = predicateDefinition[KeplerRuleSet.Name, 0].Atoms[0].Value;
                        if (predicateDefinition.TryGetChildNodeGroup(KeplerRuleSet.FactParameter, out factParameters))
                            _parameters = factParameters.Select(factParameter => new VariableDeclarationNode(factParameter[KeplerRuleSet.VariableDeclaration, 0])).ToList();
                        else
                            _parameters = new List<VariableDeclarationNode>();
                        if (_parameters.Count > 0)
                        {
                            assertion.Add(new OperandElement.Accessor(new QualifiedIdentifierNode(_parameters[0].VariableName)));
                            assertion.Add(_CreateRelationOperator(factParameters[0][KeplerRuleSet.ArithmeticRelation, 0]));
                            assertion.Add(new OperandElement.Constant(factParameters[0][KeplerRuleSet.ArithmeticConstant, 0]));

                            for (int parameterIndex = 1; parameterIndex < _parameters.Count; parameterIndex++)
                            {
                                assertion.Add(new OperatorElement(OperationName.Conjuction));
                                assertion.Add(new OperandElement.Accessor(new QualifiedIdentifierNode(_parameters[parameterIndex].VariableName)));
                                assertion.Add(_CreateRelationOperator(factParameters[parameterIndex][KeplerRuleSet.ArithmeticRelation, 0]));
                                assertion.Add(new OperandElement.Constant(factParameters[parameterIndex][KeplerRuleSet.ArithmeticConstant, 0]));
                            }
                        }

                        _body = new BodyNode(new ExitStatementNode(new ExpressionNode(assertion)));
                    }
                    else
                        throw new ArgumentException("Expected predicate definition or fact definition node!", "predicateDefinition");
            else
                throw new ArgumentNullException("predicateDefinition");
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string FullName
        {
            get
            {
                return string.Format("{0}({1})", _name, string.Join(", ", _parameters.Select(parameter => parameter.Type.ToString())));
            }
        }

        public BodyNode Body
        {
            get
            {
                return _body;
            }
        }

        public IReadOnlyList<VariableDeclarationNode> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public bool IsPredicate
        {
            get
            {
                return !_isFact;
            }
        }

        public bool IsFact
        {
            get
            {
                return _isFact;
            }
        }

        private readonly bool _isFact;
        private readonly string _name;
        private readonly BodyNode _body;
        private readonly IReadOnlyList<VariableDeclarationNode> _parameters;
    }
}
