using System;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.TerminalSymbols
{
    public sealed class IdentifierSymbol
        : TerminalSymbol
    {
        // refactor IdentifierSymbol.TryCreate
        public static bool TryCreate(ScannedAtom<Lexicon> atom, out IdentifierSymbol identifierSymbol)
        {
            try
            {
                identifierSymbol = new IdentifierSymbol(atom);
            }
            catch
            {
                identifierSymbol = null;
            }

            return (identifierSymbol != null);
        }

        public IdentifierSymbol(ScannedAtom<Lexicon> atom)
            : base(SymbolNodeType.Identifier)
        {
            if (atom != null)
                if (atom.Code == Lexicon.Identifier
                    && atom.HasValue)
                {
                    _symbol = atom.Value;
                    _atom = atom;
                }
                else
                    throw ExceptionFactory.CreateExpectedAtom("identifier", atom.Line, atom.Column);
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
