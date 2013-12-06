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
        internal ProgramNode(ParsedNode<Lexicon> programParsedNode)
        {
            if (programParsedNode != null)
                if (programParsedNode.Name == KeplerRuleSet.Program)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childNodes;
                    List<PredicateDeclarationNode> predicateDeclarationNodes = new List<PredicateDeclarationNode>();

                    if (programParsedNode.TryGetChildNodeGroup(KeplerRuleSet.PredicateDefinition, out childNodes))
                        foreach (ParsedNode<Lexicon> predicateDefinition in childNodes)
                            predicateDeclarationNodes.Add(new PredicateDeclarationNode(predicateDefinition));
                    if (programParsedNode.TryGetChildNodeGroup(KeplerRuleSet.FactDefinition, out childNodes))
                        foreach (ParsedNode<Lexicon> factDefinition in childNodes)
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
