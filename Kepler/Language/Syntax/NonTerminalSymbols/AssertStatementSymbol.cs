using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class AssertStatementSymbol
        : NonTerminalSymbol
    {
        public AssertStatementSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.AssertStatement)
        {
            Exception exception = Validate(atoms, startIndex, 2);

            if (exception == null)
            {
                List<Symbol> symbols = new List<Symbol> { new AssertSymbol(atoms[startIndex], language) };

                _expression = new ExpressionSymbol(atoms, language, startIndex + 1);
                symbols.AddRange(_expression.Symbols);
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

        public ExpressionSymbol Expression
        {
            get
            {
                return _expression;
            }
        }

        private readonly ExpressionSymbol _expression;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
