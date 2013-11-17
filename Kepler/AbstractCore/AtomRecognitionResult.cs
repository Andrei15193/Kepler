using System;

namespace Andrei15193.Kepler.AbstractCore
{
    public struct AtomRecognitionResult<TCode>
        where TCode : struct
    {
        public AtomRecognitionResult(string sequence, TCode code)
        {
            if (sequence != null)
            {
                _sequence = sequence;
                _code = code;
                _success = true;
            }
            else
                throw new ArgumentNullException("sequence");
        }

        public string Sequence
        {
            get
            {
                return _sequence;
            }
        }

        public TCode Code
        {
            get
            {
                return _code;
            }
        }

        public bool Success
        {
            get
            {
                return _success;
            }
        }

        private readonly bool _success;
        private readonly TCode _code;
        private readonly string _sequence;
    }
}
