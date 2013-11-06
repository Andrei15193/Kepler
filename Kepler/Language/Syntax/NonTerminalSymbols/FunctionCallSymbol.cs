using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class FunctionCallSymbol
        : NonTerminalSymbol
    {
        // refactor FunctionCallSymbol.TryCreate
        public static bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out FunctionCallSymbol functionCallSymbol)
        {
            try
            {
                functionCallSymbol = new FunctionCallSymbol(atoms, language);
            }
            catch
            {
                functionCallSymbol = null;
            }

            return (functionCallSymbol != null);
        }

        public FunctionCallSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.FunctionCall)
        {

        }

        public override IReadOnlyList<Symbol> Symbols
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override uint Line
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override uint Column
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
