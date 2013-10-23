namespace Andrei15193.Kepler.Compiler.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class LiteralAtomAttribute : AtomAttribute
    {
        public LiteralAtomAttribute(string literal, AtomAttribute.LiteralType atomType, bool isReservedWord, bool consider = true)
        {
            if (literal != null)
                if (literal.Length > 0)
                {
                    _literal = literal;
                    _atomType = atomType;
                    _isReservedWord = isReservedWord;
                    _considerAtom = consider;
                }
                else
                    throw new ArgumentException("The length of the literal cannot be zero.");
            else
                throw new ArgumentNullException("literal");
        }

        public string Literal
        {
            get
            {
                return _literal;
            }
        }

        public AtomAttribute.LiteralType AtomType
        {
            get
            {
                return _atomType;
            }
        }

        public bool IsReservedWord
        {
            get
            {
                return _isReservedWord;
            }
        }

        public bool ConsiderAtom
        {
            get
            {
                return _considerAtom;
            }
        }

        private readonly bool _isReservedWord;
        private readonly bool _considerAtom;
        private readonly AtomAttribute.LiteralType _atomType;
        private readonly string _literal;
    }
}
