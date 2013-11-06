namespace Andrei15193.Kepler.Language.Syntax
{
    public abstract class TerminalSymbol
        : Symbol
    {
        protected TerminalSymbol(SymbolNodeType symbolNodeType)
            : base(symbolNodeType, isTerminal: true)
        {
        }

        public abstract string Symbol
        {
            get;
        }

        public sealed override string ToString()
        {
            return "{[Terminal]" + Symbol + "}";
        }
    }
}
