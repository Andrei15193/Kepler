using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class FunctionCallSymbol
        : NonTerminalSymbol
    {
        // refactor FunctionCallSymbol.TryCreate
        public static bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out FunctionCallSymbol functionCallSymbol, int startIndex = 0)
        {
            try
            {
                functionCallSymbol = new FunctionCallSymbol(atoms, language, startIndex);
            }
            catch
            {
                functionCallSymbol = null;
            }

            return (functionCallSymbol != null);
        }

        public FunctionCallSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.FunctionCall)
        {
            Exception exception = Validate(atoms, startIndex, 3);

            if (exception == null)
                if (language != null)
                {
                    int functionNameStartIndex = startIndex;
                    NewSymbol newSymbol;
                    List<Symbol> symbols = new List<Symbol>();

                    if (NewSymbol.TryCreate(atoms[0], language, out newSymbol))
                    {
                        symbols.Add(newSymbol);
                        startIndex++;
                    }
                    _functionName = new QualifiedIdentifierSymbol(atoms, language, startIndex);
                    symbols.AddRange(_functionName.Symbols);
                    _parameters = new FuncitonParametersSymbol(atoms, language, symbols.Count);
                    symbols.AddRange(_parameters.Symbols);

                    _symbols = new ReadOnlyCollection<Symbol>(symbols);
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

        public bool IsConstructorCall
        {
            get
            {
                return (_symbols[0].SymbolNodeType == SymbolNodeType.New);
            }
        }

        public QualifiedIdentifierSymbol FunctionName
        {
            get
            {
                return _functionName;
            }
        }

        public FuncitonParametersSymbol Parameters
        {
            get
            {
                return _parameters;
            }
        }

        private readonly QualifiedIdentifierSymbol _functionName;
        private readonly FuncitonParametersSymbol _parameters;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
