using System;
using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public class Identifier
        : Product
    {
        static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, out int length, out Identifier identifier, int startIndex = 0)
        {
            length = 0;
            identifier = null;

            if (Validate(atoms, startIndex, 1) == null)
            {
                ScannedAtom<Lexicon> scannedAtom = atoms[startIndex];

                if (scannedAtom.Code == Lexicon.Identifier && scannedAtom.HasValue)
                {
                    identifier = new Identifier(scannedAtom);
                    length = 1;
                }
            }

            return (identifier != null);
        }

        public Identifier(IReadOnlyList<ScannedAtom<Lexicon>> atoms, out int length, int startIndex = 0)
            : base(ProductType.Identifier, isTerminal: true)
        {
            Exception exception = Validate(atoms, startIndex, 1);

            if (exception != null)
            {
                ScannedAtom<Lexicon> scannedAtom = atoms[startIndex];

                if (scannedAtom.Code == Lexicon.Identifier && scannedAtom.HasValue)
                {
                    _atom = scannedAtom;
                    length = 1;
                }
                else
                    throw ExceptionFactory.CreateExpectedAtom("identifier", scannedAtom.Line, scannedAtom.Column);
            }
            else
                throw exception;
        }

        protected Identifier(ScannedAtom<Lexicon> atom, ProductType productType = ProductType.Identifier)
            : base(productType, isTerminal: true)
        {
            if (atom != null)
                if (atom.Code == Lexicon.Identifier
                    && atom.HasValue)
                    _atom = atom;
                else
                    throw ExceptionFactory.CreateExpectedAtom(Lexicon.Identifier.ToString(), atom.Line, atom.Column);
            else
                throw new ArgumentNullException("atom");
        }

        public string Name
        {
            get
            {
                return _atom.Value;
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

        private readonly ScannedAtom<Lexicon> _atom;
    }
}
