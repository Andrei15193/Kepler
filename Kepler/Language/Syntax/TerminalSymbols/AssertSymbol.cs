using System;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.TerminalSymbols
{
    public sealed class AssertSymbol
        : TerminalSymbol
    {
        public AssertSymbol(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language)
            : base(SymbolNodeType.Assert)
        {
            if (atom != null)
                if (language != null)
                    if (atom.Code == Lexicon.Assert)
                    {
                        _symbol = language.GetSymbol(atom.Code);
                        _atom = atom;
                    }
                    else
                        throw ExceptionFactory.CreateExpectedSymbol("assert", atom.Line, atom.Column);
                else
                    throw new ArgumentNullException("language");
            else
                throw new ArgumentNullException("atom");
        }

        public override string Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public override uint Line
        {
            get
            {
                return _atom.Line;
            }
        }

        public override uint Column
        {
            get
            {
                return _atom.Column;
            }
        }

        private readonly string _symbol;
        private readonly ScannedAtom<Lexicon> _atom;
    }
}
