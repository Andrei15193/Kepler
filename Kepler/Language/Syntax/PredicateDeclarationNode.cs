using System;
using System.Collections.Generic;
using System.Linq;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public class PredicateDeclarationNode
    {
        public PredicateDeclarationNode(ParsedNode<Lexicon> predicateDefinition)
        {
            if (predicateDefinition != null)
                if (predicateDefinition.Name == KeplerRuleSet.PredicateDefinition)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> variableDeclarations;

                    _isFact = false;
                    _name = predicateDefinition[KeplerRuleSet.Name, 0].Atoms[0].Value;

                    if (predicateDefinition.TryGetChildNodeGroup(KeplerRuleSet.VariableDeclaration, out variableDeclarations))
                        _parameterTypes = variableDeclarations.Select(variableDeclaration => new TypeNode(variableDeclaration[KeplerRuleSet.Type, 0])).ToList();
                }
                else
                    if (predicateDefinition.Name == KeplerRuleSet.FactDefinition)
                    {
                        IReadOnlyList<ParsedNode<Lexicon>> factParameters;

                        _isFact = true;
                        _name = predicateDefinition[KeplerRuleSet.Name, 0].Atoms[0].Value;
                        if (predicateDefinition.TryGetChildNodeGroup(KeplerRuleSet.FactParameter, out factParameters))
                            _parameterTypes = factParameters.Select(factParameter => new TypeNode(factParameter[KeplerRuleSet.Type, 0])).ToList();
                    }
                    else
                        throw new ArgumentException("Must be predicate definition or fact definition parsed node!", "predicateDefinition");
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
                return string.Format("{0}({1})", _name, string.Join(", ", _parameterTypes.Select(parameterType => parameterType.ToString())));
            }
        }

        public IReadOnlyList<TypeNode> ParameterTypes
        {
            get
            {
                return _parameterTypes;
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
        private readonly IReadOnlyList<TypeNode> _parameterTypes = new List<TypeNode>();
    }
}
