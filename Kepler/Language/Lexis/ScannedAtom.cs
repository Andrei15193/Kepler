using System;

namespace Andrei15193.Kepler.Language.Lexis
{
    [Serializable]
    public sealed class ScannedAtom<TCode>
        where TCode : struct
    {
        public ScannedAtom(TCode code, uint line, uint column, string value = null)
        {
            _code = code;
            _line = line;
            _column = column;
            _value = value;
        }

        public TCode Code
        {
            get
            {
                return _code;
            }
        }

        public uint Line
        {
            get
            {
                return _line;
            }
        }

        public uint Column
        {
            get
            {
                return _column;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public bool HasValue
        {
            get
            {
                return (_value != null);
            }
        }

        private readonly uint _line;
        private readonly uint _column;
        private readonly TCode _code;
        private readonly string _value;
    }
}
