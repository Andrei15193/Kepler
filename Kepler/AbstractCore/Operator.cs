namespace Andrei15193.Kepler.AbstractCore
{
    public sealed class Operator<TCode>
        where TCode : struct
    {
        public Operator(TCode code, int priority)
        {
            _code = code;
            _priority = priority;
        }

        public TCode Code
        {
            get
            {
                return _code;
            }
        }

        public int Priority
        {
            get
            {
                return _priority;
            }
        }

        private readonly TCode _code;
        private readonly int _priority;
    }
}
