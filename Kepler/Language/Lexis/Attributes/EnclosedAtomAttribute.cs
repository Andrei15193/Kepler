using System;

namespace Andrei15193.Kepler.Language.Lexis.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class EnclosedAtomAttribute
        : AtomAttribute
    {
        public EnclosedAtomAttribute(string openingSymbol, string closingSymbol, AtomAttribute.EnclosureType atomType)
        {
            if (openingSymbol != null)
                if (closingSymbol != null)
                    if (openingSymbol.Length > 0)
                        if (closingSymbol.Length > 0)
                        {
                            _openingSymbol = openingSymbol;
                            _closingSymbol = closingSymbol;
                            _atomType = atomType;
                            _innerSequenceLength = null;
                        }
                        else
                            throw new ArgumentException("The length of the closingSymbol cannot be zero.");
                    else
                        throw new ArgumentException("The length of the openingSymbol cannot be zero.");
                else
                    throw new ArgumentNullException("closingSymbol");
            else
                throw new ArgumentNullException("openingSymbol");
        }

        public EnclosedAtomAttribute(string openingSymbol, string closingSymbol, AtomAttribute.EnclosureType atomType, uint innerSequenceLength)
            : this(openingSymbol, closingSymbol, atomType)
        {
            _innerSequenceLength = innerSequenceLength;
        }

        public string OpeningSymbol
        {
            get
            {
                return _openingSymbol;
            }
        }

        public string ClosingSymbol
        {
            get
            {
                return _closingSymbol;
            }
        }

        public AtomAttribute.EnclosureType AtomType
        {
            get
            {
                return _atomType;
            }
        }

        public uint? InnerSequenceLength
        {
            get
            {
                return _innerSequenceLength;
            }
        }

        private readonly uint? _innerSequenceLength;
        private readonly AtomAttribute.EnclosureType _atomType;
        private readonly string _openingSymbol;
        private readonly string _closingSymbol;
    }
}
