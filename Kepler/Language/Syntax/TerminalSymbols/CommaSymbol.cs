using System;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.TerminalSymbols
{
    public sealed class CommaSymbol
        : TerminalSymbol
    {
        // CommaSymbol.TryCreate
        public static bool TryCreate(ScannedAtom<Lexicon> scannedAtom, ILanguage<Lexicon> language, out CommaSymbol commaSymbol)
        {
            try
            {
                commaSymbol = new CommaSymbol(scannedAtom, language);
            }
            catch
            {
                commaSymbol = null;
            }

            return (commaSymbol != null);
        }

        public CommaSymbol(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language)
            : base(SymbolNodeType.Comma)
        {
            if (atom != null)
                if (language != null)
                    if (atom.Code == Lexicon.Comma)
                    {
                        _symbol = language.GetSymbol(atom.Code);
                        _atom = atom;
                    }
                    else
                        throw ExceptionFactory.CreateExpectedSymbol(",", atom.Line, atom.Column);
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
