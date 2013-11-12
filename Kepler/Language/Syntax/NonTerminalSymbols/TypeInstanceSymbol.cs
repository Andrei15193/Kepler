using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class TypeInstanceSymbol
        : NonTerminalSymbol
    {
        public TypeInstanceSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.TypeInstance)
        {
            Exception exception = Validate(atoms, startIndex, 1);

            if (exception == null)
            {
                _typeName = new QualifiedIdentifierSymbol(atoms, language, startIndex);
                if (atoms[startIndex + _typeName.Symbols.Count].Code == Lexicon.LessThan)
                    _genericParameters = new GenericParametersSymbol(atoms, language, startIndex + _typeName.Symbols.Count);
                else
                    _genericParameters = new GenericParametersSymbol();

                int currentLength = _typeName.Symbols.Count + _genericParameters.Symbols.Count;
                IList<BoundedArray> arraySymbols = new List<BoundedArray>();
                List<Symbol> symbols = new List<Symbol>();
                symbols.AddRange(_typeName.Symbols);
                symbols.AddRange(_genericParameters.Symbols);

                while (startIndex + currentLength < atoms.Count && atoms[startIndex + currentLength].Code == Lexicon.OpeningSquareParenthesis)
                {
                    BoundedArray arraySymbol = new BoundedArray(atoms, language, currentLength);
                    symbols.AddRange(arraySymbol.Symbols);
                    arraySymbols.Add(arraySymbol);
                    currentLength += arraySymbol.Symbols.Count;
                }

                _arraySymbols = new ReadOnlyCollection<BoundedArray>(arraySymbols);
                _symbols = new ReadOnlyCollection<Symbol>(symbols);
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

        public QualifiedIdentifierSymbol TypeName
        {
            get
            {
                return _typeName;
            }
        }

        public GenericParametersSymbol GenericParameters
        {
            get
            {
                return _genericParameters;
            }
        }

        public IReadOnlyList<BoundedArray> Arrays
        {
            get
            {
                return _arraySymbols;
            }
        }

        private readonly QualifiedIdentifierSymbol _typeName;
        private readonly GenericParametersSymbol _genericParameters;
        private readonly IReadOnlyList<Symbol> _symbols;
        private readonly IReadOnlyList<BoundedArray> _arraySymbols;

        internal static bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out TypeInstanceSymbol typeInstanceSymbol, int p)
        {
            throw new NotImplementedException();
        }
    }
}
