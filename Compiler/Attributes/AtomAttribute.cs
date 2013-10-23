namespace Andrei15193.Kepler.Compiler.Attributes
{
    using System;

    public class AtomAttribute : Attribute
    {
        public enum PatternType
        {
            Identifier,
            Constant,
            Ignore
        }

        public enum LiteralType
        {
            KeyWord,
            Separator,
            Operator
        }

        public enum EnclosureType
        {
            Constant,
            Comment
        }

        protected AtomAttribute()
        {
        }
    }
}
