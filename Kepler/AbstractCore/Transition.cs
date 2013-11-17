using System;

namespace Andrei15193.Kepler.AbstractCore
{
    public sealed class Transition<T>
    {
        public Transition(Func<T, bool> condition, State<T> destination)
        {
            if (destination != null)
            {
                _condition = condition;
                _destination = destination;
            }
            else
                throw new ArgumentNullException("destination");
        }

        public bool CanTransit(T item)
        {
            return (_condition == null || _condition(item));
        }

        public State<T> Destination
        {
            get
            {
                return _destination;
            }
        }

        private readonly Func<T, bool> _condition;
        private readonly State<T> _destination;
    }
}
