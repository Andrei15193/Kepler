using System;

namespace Andrei15193.Kepler.Language.Lexis.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class PriorityAttribute : Attribute
    {
        static public PriorityAttribute Default
        {
            get
            {
                return _defaultPriorityAttribute;
            }
        }

        public PriorityAttribute(int priority = 0)
        {
            _priority = priority;
        }

        public int Priority
        {
            get
            {
                return _priority;
            }
        }

        static private readonly PriorityAttribute _defaultPriorityAttribute = new PriorityAttribute();
        private readonly int _priority;
    }
}
