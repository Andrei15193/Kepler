using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class WhileStatementSymbol
        : NonTerminalSymbol
    {
        static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out WhileStatementSymbol whileStatementSymbol, int startIndex = 0)
        {
            try
            {
                whileStatementSymbol = new WhileStatementSymbol(atoms, language, startIndex);
            }
            catch
            {
                whileStatementSymbol = null;
            }

            return (whileStatementSymbol != null);
        }

        public WhileStatementSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.WhileStatement)
        {
            Exception exception = Validate(atoms, startIndex, 5);

            if (exception == null)
            {
                List<Symbol> symbols = new List<Symbol>                {                    new WhileSymbol(atoms[startIndex], language)                };

                _condition = new ExpressionSymbol(atoms, language, startIndex + 1);
                symbols.AddRange(_condition.Symbols);
                symbols.Add(new DoSymbol(atoms[symbols.Count], language));
                _body = new BodySymbol(atoms, language,symbols.Count);
                symbols.AddRange(_body.Symbols);
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

        public ExpressionSymbol Condition
        {
            get
            {
                return _condition;
            }
        }

        public BodySymbol Body
        {
            get
            {
                return _body;
            }
        }

        private readonly ExpressionSymbol _condition;
        private readonly BodySymbol _body;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
