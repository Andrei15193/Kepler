using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class BodySymbol
        : NonTerminalSymbol
    {
        public BodySymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.Body)
        {
            Exception exception = Validate(atoms, startIndex, 2);

            if (exception == null)
            {
                List<Symbol> symbols = new List<Symbol>();
                IList<StatementSymbol> statements = new List<StatementSymbol>();

                if (atoms[startIndex].Code == Lexicon.Begin)
                {
                    int currentIndex = startIndex + 1;

                    _isBlock = true;
                    symbols.Add(new BeginSymbol(atoms[startIndex], language));
                    while (currentIndex < atoms.Count
                           && atoms[currentIndex].Code != Lexicon.End)
                    {
                        statements.Add(new StatementSymbol(atoms, language, currentIndex));
                        currentIndex += statements[statements.Count - 1].Symbols.Count;
                        symbols.AddRange(statements[statements.Count - 1].Symbols);
                    }
                    symbols.Add(new EndSymbol(atoms[currentIndex], language));
                }
                else
                {
                    _isBlock = false;
                    statements.Add(new StatementSymbol(atoms, language, startIndex));
                    symbols.AddRange(statements[statements.Count - 1].Symbols);
                }

                _statements = new ReadOnlyCollection<StatementSymbol>(statements);
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

        public IReadOnlyList<StatementSymbol> Statements
        {
            get
            {
                return _statements;
            }
        }

        public bool IsBlock
        {
            get
            {
                return _isBlock;
            }
        }

        private readonly bool _isBlock;
        private readonly IReadOnlyList<Symbol> _symbols;
        private readonly IReadOnlyList<StatementSymbol> _statements;
    }
}
