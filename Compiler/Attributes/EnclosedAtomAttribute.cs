namespace Andrei15193.Kepler.Compiler.Attributes
{
    using System;

    public class EnclosedAtomAttribute : AtomAttribute
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

        private readonly AtomAttribute.EnclosureType _atomType;
        private readonly string _openingSymbol;
        private readonly string _closingSymbol;
    }
}
