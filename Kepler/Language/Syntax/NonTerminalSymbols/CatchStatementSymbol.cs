using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class CatchStatementSymbol
        : NonTerminalSymbol
    {
        static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out CatchStatementSymbol catchStatementSymbol, int startIndex = 0)
        {
            try
            {
                catchStatementSymbol = new CatchStatementSymbol(atoms, language, startIndex);
            }
            catch
            {
                catchStatementSymbol = null;
            }

            return (catchStatementSymbol != null);
        }

        public CatchStatementSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.CatchStatement)
        {
            Exception exception = Validate(atoms, startIndex, 3);

            if (exception == null)
            {
                List<Symbol> symbols = new List<Symbol> { new CatchSymbol(atoms[startIndex], language) };

                if (VariableDeclarationtSymbol.TryCreate(atoms, language, out _exception, startIndex + 1))
                    symbols.AddRange(_exception.Symbols);
                else
                    _exception = null;

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

        public VariableDeclarationtSymbol Exception
        {
            get
            {
                return _exception;
            }
        }

        public BodySymbol Body
        {
            get
            {
                return _body;
            }
        }

        public bool HasException
        {
            get
            {
                return (_exception != null);
            }
        }

        private readonly VariableDeclarationtSymbol _exception;
        private readonly BodySymbol _body;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
