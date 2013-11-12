using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class QualifiedIdentifierSymbol
        : NonTerminalSymbol
    {
        // refactor QualifiedIdentifierSymbol.TryCreate
        public static bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out QualifiedIdentifierSymbol qualifiedIdentifierSymbol, int startIndex = 0)
        {
            try
            {
                qualifiedIdentifierSymbol = new QualifiedIdentifierSymbol(atoms, language, startIndex);
            }
            catch
            {
                qualifiedIdentifierSymbol = null;
            }

            return (qualifiedIdentifierSymbol != null);
        }

        public QualifiedIdentifierSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.QualifiedIdentifier)
        {
            Exception exception = Validate(atoms, startIndex, 1);

            if (exception == null)
                if (language != null)
                {
                    IList<Symbol> symbols = new List<Symbol>() { new IdentifierSymbol(atoms[startIndex]) };
                    int currentIndex = startIndex + 2;

                    while (currentIndex < atoms.Count
                           && atoms[currentIndex - 1].Code == Lexicon.Scope
                           && atoms[currentIndex].Code == Lexicon.Identifier)
                    {
                        symbols.Add(new ScopeSymbol(atoms[currentIndex - 1], language));
                        symbols.Add(new IdentifierSymbol(atoms[currentIndex]));
                        currentIndex += 2;
                    }

                    _symbols = new ReadOnlyCollection<Symbol>(symbols);
                    _identifiers = new ReadOnlyCollection<string>(symbols.OfType<IdentifierSymbol>().Select(symbol => symbol.Symbol).ToList());
                }
                else
                    throw new ArgumentNullException("language");
            else
                throw exception;
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

        public IReadOnlyList<string> Identifiers
        {
            get
            {
                return _identifiers;
            }
        }

        private readonly IReadOnlyList<Symbol> _symbols;
        private readonly IReadOnlyList<string> _identifiers;
    }
}
