using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class ProgramSymbol
        :NonTerminalSymbol
    {
        public ProgramSymbol(LexicalAnalysisResult<Lexicon> lexicalAnalysisResult, ILanguage<Lexicon> language)
            : base (SymbolNodeType.Program)
        {
            if (lexicalAnalysisResult != null)
                if (language != null)
                {
                    List<Symbol> symbols = new List<Symbol>();
                    List<PredicateDeclarationSymbol> predicates = new List<PredicateDeclarationSymbol>();

                    while (symbols.Count < lexicalAnalysisResult.ScannedAtoms.Count)
                    {
                        PredicateDeclarationSymbol predicate = new PredicateDeclarationSymbol(lexicalAnalysisResult.ScannedAtoms, language, symbols.Count);

                        symbols.AddRange(predicate.Symbols);
                        predicates.Add(predicate);
                    }

                    _predicates = new ReadOnlyCollection<PredicateDeclarationSymbol>(predicates);
                    _symbols = new ReadOnlyCollection<Symbol>(symbols);
                }
                else
                    throw new ArgumentNullException("language");
            else
                throw new ArgumentNullException("atoms");
        }

        public override IReadOnlyList<Symbol> Symbols
        {
            get
            {
                return _symbols;
            }
        }

        public override uint Line
        {
            get
            {
                return _symbols[0].Line;
            }
        }

        public override uint Column
        {
            get
            {
                return _symbols[0].Column;
            }
        }

        public IReadOnlyList<PredicateDeclarationSymbol> Predicates
        {
            get
            {
                return _predicates;
            }
        }

        private IReadOnlyList<PredicateDeclarationSymbol> _predicates;
        private IReadOnlyList<Symbol> _symbols;
    }
}
