using System;

namespace Andrei15193.Kepler.Language.Attributes
{
    public class AtomAttribute
        : Attribute
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
