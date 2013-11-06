using System;
using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public abstract class Symbol
    {
        static protected class ExceptionFactory
        {
            static public Exception CreateExpectedSymbol(string symbol, uint line, uint column)
            {
                if (symbol != null)
                    if (symbol.Length > 0)
                        return new ArgumentException(string.Format("Expected '{0}' at line: {1}, column: {2}", symbol, line, column));
                    else
                        throw new ArgumentException("The symbol cannot be missing (empty string)!");
                else
                    throw new ArgumentNullException("symbol");
            }

            static public Exception CreateUnexpectedSymbol(string symbol, uint line, uint column)
            {
                if (symbol != null)
                    if (symbol.Length > 0)
                        return new ArgumentException(string.Format("Unexpected '{0}' at line: {1}, column: {2}", symbol, line, column));
                    else
                        throw new ArgumentException("The symbol cannot be missing (empty string)!");
                else
                    throw new ArgumentNullException("symbol");
            }

            static public Exception CreateExpectedAtom(string atomType, uint line, uint column)
            {
                if (atomType != null)
                {
                    string trimmedAtomType = atomType.Trim();

                    if (trimmedAtomType.Length > 0)
                        return new ArgumentException(string.Format("Expected {0} at line: {1}, column: {2}", atomType, line, column));
                    else
                        throw new ArgumentException("The atom type cannot be white space only or missing (empty string)!");
                }
                else
                    throw new ArgumentNullException("atomType");
            }
        }

        static protected Exception Validate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, int startIndex)
        {
            if (atoms == null)
                throw new ArgumentNullException("atoms");
            else
                if (startIndex < 0 || atoms.Count <= startIndex)
                    return new ArgumentOutOfRangeException("startIndex");
                else
                    return null;
        }

        static protected Exception Validate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, int startIndex, int minimumLength)
        {
            Exception exception = Validate(atoms, startIndex);

            if (exception == null)
            {
                if (minimumLength < 0)
                    exception = new ArgumentException("minimumLength cannot be negative!");
                else
                    if (atoms.Count - startIndex < minimumLength)
                        exception = new ArgumentException("The length of the provided list must be at least " + minimumLength);
            }

            return exception;
        }

        protected Symbol(SymbolNodeType symbolNodeType, bool isTerminal)
        {
            _symbolNodeType = symbolNodeType;
            _isTerminal = isTerminal;
        }

        public SymbolNodeType SymbolNodeType
        {
            get
            {
                return _symbolNodeType;
            }
        }

        public abstract uint Line
        {
            get;
        }

        public abstract uint Column
        {
            get;
        }

        public bool IsTerminal
        {
            get
            {
                return _isTerminal;
            }
        }

        private readonly bool _isTerminal;
        private readonly SymbolNodeType _symbolNodeType;
    }
}
