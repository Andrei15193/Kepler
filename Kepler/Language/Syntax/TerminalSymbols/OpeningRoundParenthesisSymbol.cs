using System;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.TerminalSymbols
{
    public sealed class OpeningRoundParenthesisSymbol
        : TerminalSymbol
    {
        // refactor OpeningRoundParenthesisSymbol.TryCreate
        static public bool TryCreate(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language, out OpeningRoundParenthesisSymbol result)
        {
            try
            {
                result = new OpeningRoundParenthesisSymbol(atom, language);
            }
            catch
            {
                result = null;
            }

            return (result != null);
        }

        public OpeningRoundParenthesisSymbol(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language)
            : base(SymbolNodeType.OpeningRoundParenthesis)
        {
            if (atom != null)
                if (language != null)
                    if (atom.Code == Lexicon.OpeningRoundParenthesis)
                    {
                        _symbol = language.GetSymbol(atom.Code);
                        _atom = atom;
                    }
                    else
                        throw ExceptionFactory.CreateExpectedSymbol("(", atom.Line, atom.Column);
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
