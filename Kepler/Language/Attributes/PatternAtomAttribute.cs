using System;
using System.Text.RegularExpressions;

namespace Andrei15193.Kepler.Language.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class PatternAtomAttribute
        : AtomAttribute
    {
        public PatternAtomAttribute(string pattern, RegexOptions patternOptions, AtomAttribute.PatternType atomType)
        {
            if (pattern != null)
                if (pattern.Length > 0)
                {
                    _atomType = atomType;
                    _regex = new Regex(pattern, patternOptions);
                }
                else
                    throw new ArgumentException("The length of the pattern cannot be zero.");
            else
                throw new ArgumentNullException("pattern");
        }

        public Regex Regex
        {
            get
            {
                return _regex;
            }
        }

        public AtomAttribute.PatternType AtomType
        {
            get
            {
                return _atomType;
            }
        }

        private readonly AtomAttribute.PatternType _atomType;
        private readonly Regex _regex;
    }
}
