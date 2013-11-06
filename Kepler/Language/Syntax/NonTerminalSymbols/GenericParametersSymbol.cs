using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class GenericParametersSymbol
        : NonTerminalSymbol
    {
        public GenericParametersSymbol()
            : base(SymbolNodeType.GenericParameters)
        {
            _symbols = new ReadOnlyCollection<Symbol>(new Symbol[0]);
            _typeSymbols = new ReadOnlyCollection<TypeSymbol>(new TypeSymbol[0]);
        }

        public GenericParametersSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.GenericParameters)
        {
            Exception exception = Validate(atoms, startIndex, 3);

            if (exception == null)
            {
                List<Symbol> symbols = new List<Symbol>() { new OpeningAngleBracketSymbol(atoms[startIndex], language) };
                IList<TypeSymbol> typeSymbols = new List<TypeSymbol>() { new TypeSymbol(atoms, language, startIndex + symbols.Count) };
                symbols.AddRange(typeSymbols[0].Symbols);

                while (startIndex + symbols.Count < atoms.Count && atoms[startIndex + symbols.Count].Code != Lexicon.GreaterThan)
                {
                    symbols.Add(new CommaSymbol(atoms[startIndex + symbols.Count], language));

                    TypeSymbol typeSymbol = new TypeSymbol(atoms, language, startIndex + symbols.Count);

                    typeSymbols.Add(typeSymbol);
                    symbols.AddRange(typeSymbol.Symbols);
                }

                if (startIndex + symbols.Count < atoms.Count)
                {
                    symbols.Add(new ClosingAngleBracketSymbol(atoms[startIndex + symbols.Count], language));
                    _symbols = new ReadOnlyCollection<Symbol>(symbols);
                    _typeSymbols = new ReadOnlyCollection<TypeSymbol>(typeSymbols);
                }
                else
                    throw ExceptionFactory.CreateExpectedSymbol(">", atoms[atoms.Count - 1].Line, atoms[atoms.Count - 1].Column);
            }
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

        public IReadOnlyList<TypeSymbol> Types
        {
            get
            {
                return _typeSymbols;
            }
        }

        private readonly IReadOnlyList<Symbol> _symbols;
        private readonly IReadOnlyList<TypeSymbol> _typeSymbols;
    }
}
