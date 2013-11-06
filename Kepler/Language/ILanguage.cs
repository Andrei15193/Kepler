using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language
{
    public interface ILanguage<TCode>
        where TCode : struct
    {
        bool TryGetIdentifierCode(string text, out TCode code);

        bool TryGetConstantCode(string text, out TCode code);

        bool TryGetKeyWordCode(string text, out TCode code);

        bool TryGetIgnoreCode(string text, out TCode code);

        bool CanIgnore(string text);

        bool CanIgnore(TCode code);

        string GetSymbol(TCode code);

        bool TryGetSymbol(TCode code, out string symbol);

        bool IsReservedWord(string text);
        
        IReadOnlyDictionary<string, Operator<TCode>> Operators
        {
            get;
        }

        IReadOnlyDictionary<string, TCode> Separators
        {
            get;
        }

        IEnumerable<string> ReservedWords
        {
            get;
        }

        IEnumerable<Enclosure> Enclosures
        {
            get;
        }
    }
}
