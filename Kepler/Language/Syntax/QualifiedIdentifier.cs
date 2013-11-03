using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Extensions;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class QualifiedIdentifier
        : Identifier
    {
        new static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, out int length, out QualifiedIdentifier identifier, int startIndex = 0)
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
                    length =  currentIndex - startIndex - 2;
                    identifier = new QualifiedIdentifier(atoms.Sublist(startIndex, length));
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
                        if (!atoms[currentIndex].HasValue)
                            throw ExceptionFactory.CreateExpectedAtom("identifier", atoms[startIndex].Line, atoms[startIndex].Column);
                        else
                            currentIndex += 2;

                    return atoms.Sublist(startIndex, currentIndex - startIndex - 2);
                }
                else
                    throw ExceptionFactory.CreateExpectedAtom("identifier", atoms[startIndex].Line, atoms[startIndex].Column);
            else
                throw exception;
        }

        public QualifiedIdentifier(IReadOnlyList<ScannedAtom<Lexicon>> atoms, int startIndex = 0)
            : this(_GetQualifiedIdentifierAtoms(atoms, 0))
        {
        }

        private QualifiedIdentifier(IReadOnlyList<ScannedAtom<Lexicon>> qualifiedIdentifierAtoms)
            : base(qualifiedIdentifierAtoms[qualifiedIdentifierAtoms.Count - 1], ProductType.QualifiedIdentifier)
        {
            _atoms = qualifiedIdentifierAtoms;

            IList<string> identifiers = new List<string>();

            for (int i = 0; i < _atoms.Count; i += 2)
                identifiers.Add(_atoms[i].Value);

            _identifiers = new ReadOnlyCollection<string>(identifiers);
        }

        public IReadOnlyList<string> Identifiers
        {
            get
            {
                return _identifiers;
            }
        }

        private readonly IReadOnlyList<ScannedAtom<Lexicon>> _atoms;
        private readonly IReadOnlyList<string> _identifiers;
    }
}
