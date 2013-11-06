using System.Collections.Generic;
using System.Linq;

namespace Andrei15193.Kepler.Language.Syntax
{
    public abstract class NonTerminalSymbol
         : Symbol
    {
        protected NonTerminalSymbol(SymbolNodeType symbolNodeType)
            : base(symbolNodeType, isTerminal: false)
        {
        }

        public abstract IReadOnlyList<Symbol> Symbols
        {
            get;
        }

        public sealed override string ToString()
        {
            return ("{[NonTerminal]" + string.Join(" ", Symbols.Select(symbol => symbol.ToString())) + "}");
        }
    }
}
