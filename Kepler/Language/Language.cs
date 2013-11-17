using System.Collections.Generic;
using Andrei15193.Kepler.AbstractCore;

namespace Andrei15193.Kepler.Language
{
    public sealed class Language<TCode>
        : ILanguage<TCode>
        where TCode : struct
    {
        static public ILanguage<TCode> Default
        {
            get
            {
                return _instance;
            }
        }

        static private readonly ILanguage<TCode> _instance = new RegexLanguage<TCode>();

        private Language()
        {
        }

        public bool TryGetIdentifierCode(string text, out TCode code)
        {
            return Default.TryGetIdentifierCode(text, out code);
        }

        public bool TryGetConstantCode(string text, out TCode code)
        {
            return Default.TryGetConstantCode(text, out code);
        }

        public bool TryGetKeyWordCode(string text, out TCode code)
        {
            return Default.TryGetKeyWordCode(text, out code);
        }

        public bool TryGetIgnoreCode(string text, out TCode code)
        {
            return Default.TryGetIgnoreCode(text, out code);
        }

        public bool CanIgnore(string text)
        {
            return Default.CanIgnore(text);
        }

        public bool CanIgnore(TCode code)
        {
            return Default.CanIgnore(code);
        }

        public string GetSymbol(TCode code)
        {
            return Default.GetSymbol(code);
        }

        public bool TryGetSymbol(TCode code, out string symbol)
        {
            return Default.TryGetSymbol(code, out symbol);
        }

        public TCode GetCode(string symbol)
        {
            return Default.GetCode(symbol);
        }

        public bool IsReservedWord(string text)
        {
            return Default.IsReservedWord(text);
        }

        public IReadOnlyDictionary<string, Operator<TCode>> Operators
        {
            get
            {
                return Default.Operators;
            }
        }

        public IReadOnlyDictionary<string, TCode> Separators
        {
            get
            {
                return Default.Separators;
            }
        }

        public IEnumerable<string> ReservedWords
        {
            get
            {
                return Default.ReservedWords;
            }
        }

        public IEnumerable<Enclosure> Enclosures
        {
            get
            {
                return Default.Enclosures;
            }
        }
    }
}
