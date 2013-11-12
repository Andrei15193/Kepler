using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class ExitStatement
        : NonTerminalSymbol
    {
        static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out ExitStatement exitStatement, int startIndex = 0)
        {
            try
            {
                exitStatement = new ExitStatement(atoms, language, startIndex);
            }
            catch
            {
                exitStatement = null;
            }

            return (exitStatement != null);
        }

        public ExitStatement(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.Exit)
        {
            Exception exception = Validate(atoms, startIndex, 1);

            if (exception == null)
            {
                if (atoms[startIndex].Code == Lexicon.Stop)
                {
                    _symbols = new ReadOnlyCollection<Symbol>(new[] { new StopSymbol(atoms[startIndex], language) });
                    _isStop = true;
                }
                else
                    if (atoms[startIndex].Code == Lexicon.Skip)
                    {
                        _symbols = new ReadOnlyCollection<Symbol>(new[] { new SkipSymbol(atoms[startIndex], language) });
                        _isSkip = true;
                    }
                    else
                    {
                        _assertStatementSymbol = new AssertStatementSymbol(atoms, language, startIndex);
                        _symbols = _assertStatementSymbol.Symbols;
                    }
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

        public AssertStatementSymbol AssertStatementSymbol
        {
            get
            {
                return _assertStatementSymbol;
            }
        }

        public bool IsStop
        {
            get
            {
                return _isStop;
            }
        }

        public bool IsSkip
        {
            get
            {
                return _isSkip;
            }
        }

        public bool IsAssert
        {
            get
            {
                return (!_isStop && !_isSkip);
            }
        }

        private readonly bool _isStop = false;
        private readonly bool _isSkip = false;
        private readonly AssertStatementSymbol _assertStatementSymbol;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
