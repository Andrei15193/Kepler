using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.AbstractCore;

namespace Andrei15193.Kepler.Language.Lexis
{
    [Serializable]
    public sealed class ScanResult<TCode>
        where TCode : struct
    {
        public ScanResult(IReadOnlyList<ScannedAtom<TCode>> scannedAtoms, IReadOnlyDictionary<string, string> identifiers, IReadOnlyDictionary<string, string> constants)
        {
            if (scannedAtoms != null)
                if (identifiers != null)
                    if (constants != null)
                    {
                        _scannedAtoms = new ReadOnlyCollection<ScannedAtom<TCode>>(scannedAtoms.ToList());
                        _identifiers = new ReadOnlyDictionary<string, string>(new SortedDictionary<string, string>(identifiers.ToDictionary(entry => entry.Key, entry => entry.Value)));
                        _constants = new ReadOnlyDictionary<string, string>(new SortedDictionary<string, string>(constants.ToDictionary(entry => entry.Key, entry => entry.Value)));
                    }
                    else
                        throw new ArgumentNullException("constants");
                else
                    throw new ArgumentNullException("identifiers");
            else
                throw new ArgumentNullException("scannedAtoms");
        }

        public IReadOnlyList<ScannedAtom<TCode>> ScannedAtoms
        {
            get
            {
                return _scannedAtoms;
            }
        }

        public IReadOnlyDictionary<string, string> Constants
        {
            get
            {
                return _constants;
            }
        }

        public IReadOnlyDictionary<string, string> Identifiers
        {
            get
            {
                return _identifiers;
            }
        }

        private readonly IReadOnlyList<ScannedAtom<TCode>> _scannedAtoms;
        private readonly IReadOnlyDictionary<string, string> _identifiers;
        private readonly IReadOnlyDictionary<string, string> _constants;
    }
}
