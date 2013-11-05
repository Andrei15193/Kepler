using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Extensions;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class QualifiedIdentifier
        : Identifier
    {
        static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, out int length, out QualifiedIdentifier identifier, int startIndex = 0)
        {
            length = 0;
            identifier = null;

            if (Validate(atoms, startIndex, 1) == null
                && atoms[startIndex].Code == Lexicon.Identifier)
            {
                int currentIndex = startIndex + 2;

                while (currentIndex < atoms.Count
                        && atoms[currentIndex - 1].Code == Lexicon.Scope
                        && atoms[currentIndex].Code == Lexicon.Identifier
                        && atoms[currentIndex].HasValue)
                    currentIndex += 2;

                if (currentIndex >= atoms.Count
                    || atoms[currentIndex].Code != Lexicon.Identifier
                    || atoms[currentIndex].HasValue)
                {
                    length = currentIndex - startIndex - 1;

                    int qualifiedIdentifierIndex = 0;
                    IReadOnlyList<ScannedAtom<Lexicon>> qualifiedIdentifierAtoms = atoms.Sublist(startIndex, length);

                    while (qualifiedIdentifierIndex < qualifiedIdentifierAtoms.Count
                           && qualifiedIdentifierAtoms[qualifiedIdentifierIndex].HasValue)
                        qualifiedIdentifierIndex += 2;

                    if (qualifiedIdentifierIndex == (1 + qualifiedIdentifierAtoms.Count))
                        identifier = new QualifiedIdentifier(qualifiedIdentifierAtoms, skipValidation: true);
                }
            }

            return (identifier != null);
        }

        static private IReadOnlyList<ScannedAtom<Lexicon>> _GetQualifiedIdentifierAtoms(IReadOnlyList<ScannedAtom<Lexicon>> atoms, int startIndex = 0)
        {
            Exception exception = Validate(atoms, startIndex, 1);

            if (exception == null)
                if (atoms[startIndex].Code == Lexicon.Identifier)
                {
                    int currentIndex = startIndex + 2;

                    while (currentIndex < atoms.Count
                           && atoms[currentIndex - 1].Code == Lexicon.Scope
                           && atoms[currentIndex].Code == Lexicon.Identifier)
                        currentIndex += 2;

                    return atoms.Sublist(startIndex, currentIndex - startIndex - 2);
                }
                else
                    throw ExceptionFactory.CreateExpectedAtom("identifier", atoms[startIndex].Line, atoms[startIndex].Column);
            else
                throw exception;
        }

        public QualifiedIdentifier(IReadOnlyList<ScannedAtom<Lexicon>> atoms, int startIndex = 0)
            : this(_GetQualifiedIdentifierAtoms(atoms, 0), skipValidation: false)
        {
        }

        private QualifiedIdentifier(IReadOnlyList<ScannedAtom<Lexicon>> qualifiedIdentifierAtoms, bool skipValidation)
            : base(qualifiedIdentifierAtoms[qualifiedIdentifierAtoms.Count - 1], ProductType.QualifiedIdentifier)
        {
            _atoms = qualifiedIdentifierAtoms;

            IList<string> identifiers = new List<string>();

            if (skipValidation)
                for (int atomIndex = 0; atomIndex < _atoms.Count; atomIndex += 2)
                    identifiers.Add(_atoms[atomIndex].Value);
            else
                for (int atomIndex = 0; atomIndex < _atoms.Count; atomIndex += 2)
                    if (!_atoms[atomIndex].HasValue)
                        throw ExceptionFactory.CreateExpectedAtom("identifier", _atoms[atomIndex].Line, _atoms[atomIndex].Column);
                    else
                        identifiers.Add(_atoms[atomIndex].Value);

            _identifiers = new ReadOnlyCollection<string>(identifiers);
        }

        public IReadOnlyList<string> Identifiers
        {
            get
            {
                return _identifiers;
            }
        }

        public IReadOnlyList<string> Scopes
        {
            get
            {
                return new ReadOnlyCollection<string>(_identifiers.Take(_identifiers.Count - 1).ToList());
            }
        }

        public override uint Line
        {
            get
            {
                return _atoms[0].Line;
            }
        }

        public override uint Column
        {
            get
            {
                return _atoms[0].Column;
            }
        }

        private readonly IReadOnlyList<string> _identifiers;
        private readonly IReadOnlyList<ScannedAtom<Lexicon>> _atoms;
    }
}
