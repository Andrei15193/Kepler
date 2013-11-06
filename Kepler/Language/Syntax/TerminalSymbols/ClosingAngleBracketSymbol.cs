using System;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.TerminalSymbols
{
    public sealed class ClosingAngleBracketSymbol
        : TerminalSymbol
    {
        public ClosingAngleBracketSymbol(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language)
            : base(SymbolNodeType.ClosingAngleBracket)
        {
            if (atom != null)
                if (language != null)
                    if (atom.Code == Lexicon.GreaterThan)
                    {
                        _symbol = language.GetSymbol(atom.Code);
                        _atom = atom;
                    }
                    else
                        throw ExceptionFactory.CreateExpectedSymbol(">", atom.Line, atom.Column);
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
