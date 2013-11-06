using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class ArraySymbol
        : NonTerminalSymbol
    {
        public ArraySymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.Array)
        {
            IList<Symbol> symbols = new List<Symbol>();
            Exception exception = Validate(atoms, startIndex, 2);

            if (exception == null)
                if (language != null)
                {
                    symbols.Add(new OpeningSquareParenthesisSymbol(atoms[startIndex], language));

                    int currentIndex = startIndex + 1;

                    while (currentIndex < atoms.Count && atoms[currentIndex].Code != Lexicon.ClosingSquareParenthesis)
                    {
                        symbols.Add(new CommaSymbol(atoms[currentIndex], language));
                        currentIndex++;
                    }

                    if (currentIndex < atoms.Count)
                    {
                        symbols.Add(new ClosingSquareParenthesisSymbol(atoms[currentIndex], language));
                        _symbols = new ReadOnlyCollection<Symbol>(symbols);
                    }
                    else
                        throw ExceptionFactory.CreateExpectedSymbol("]", atoms[currentIndex - 1].Line, atoms[currentIndex - 1].Column);
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

        public int Dimension
        {
            get
            {
                return (_symbols.Count - 1);
            }
        }

        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
