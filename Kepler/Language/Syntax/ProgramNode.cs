using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class ProgramNode
    {
        public ProgramNode(ParsedNode<Lexicon> programParsedNode)
        {
            if (programParsedNode != null)
                if (programParsedNode.Name == KeplerRuleSet.Program)
                {
                    List<PredicateDeclarationNode> predicateDeclarationNodes = new List<PredicateDeclarationNode>();

                    foreach (ParsedNode<Lexicon> predicateDefinition in programParsedNode[KeplerRuleSet.PredicateDefinition])
                        predicateDeclarationNodes.Add(new PredicateDeclarationNode(predicateDefinition));
                    foreach (ParsedNode<Lexicon> factDefinition in programParsedNode[KeplerRuleSet.FactDefinition])
                        predicateDeclarationNodes.Add(new PredicateDeclarationNode(factDefinition));

                    _predicateDeclarations = predicateDeclarationNodes;
                }
                else
                    throw new ArgumentException("Must be ProgramNode!", "programParsedNode");
            else
                throw new ArgumentNullException("programParsedNode");
        }

        public IReadOnlyList<PredicateDeclarationNode> PredicateDeclarations
        {
            get
            {
                return _predicateDeclarations;
            }
        }

        private readonly IReadOnlyList<PredicateDeclarationNode> _predicateDeclarations;
    }
}
