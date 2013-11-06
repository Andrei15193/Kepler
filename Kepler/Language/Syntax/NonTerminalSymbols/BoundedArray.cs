using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class BoundedArray
        : NonTerminalSymbol
    {
        public BoundedArray(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.BoundedArray)
        {
            Exception exception = Validate(atoms, startIndex, 2);

            if (exception == null)
                if (language != null)
                {
                    IList<Symbol> symbols = new List<Symbol>();
                    IList<string> bounds = new List<string>();

                    symbols.Add(new OpeningSquareParenthesisSymbol(atoms[startIndex], language));

                    int currentIndex = startIndex + 2;

                    while (currentIndex < atoms.Count && atoms[currentIndex].Code != Lexicon.ClosingSquareParenthesis)
                    {
                        ConstantSymbol constantSymbol = new ConstantSymbol(atoms[currentIndex - 1]);

                        if (constantSymbol.IsInteger)
                        {
                            symbols.Add(constantSymbol);
                            bounds.Add(constantSymbol.Symbol);
                            symbols.Add(new CommaSymbol(atoms[currentIndex], language));
                            currentIndex += 2;
                        }
                        else
                            throw ExceptionFactory.CreateExpectedAtom("positive integer", atoms[currentIndex - 1].Line, atoms[currentIndex - 1].Column);
                    }

                    if (currentIndex < atoms.Count)
                    {
                        ConstantSymbol constantSymbol = new ConstantSymbol(atoms[currentIndex - 1]);

                        if (constantSymbol.IsInteger)
                        {
                            symbols.Add(constantSymbol);
                            bounds.Add(constantSymbol.Symbol);
                        }
                        else
                            throw ExceptionFactory.CreateExpectedAtom("positive integer", atoms[currentIndex - 1].Line, atoms[currentIndex - 1].Column);
                        symbols.Add(new ClosingSquareParenthesisSymbol(atoms[currentIndex], language));
                        _symbols = new ReadOnlyCollection<Symbol>(symbols);
                        _bounds = new ReadOnlyCollection<string>(bounds);
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
                return _bounds.Count;
            }
        }

        public IReadOnlyList<string> Bounds
        {
            get
            {
                return _bounds;
            }
        }

        private readonly IReadOnlyList<string> _bounds;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
