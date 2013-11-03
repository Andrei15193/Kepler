using System;
using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public class Product
    {
        static protected class ExceptionFactory
        {
            static public Exception CreateExpectedSymbol(string symbol, uint line, uint column)
            {
                if (symbol != null)
                    return new ArgumentException(string.Format("Expected '{0}' at line: {1}, column: {2}", symbol, line, column));
                else
                    throw new ArgumentNullException("symbol");
            }

            static public Exception CreateExpectedAtom(string atomType, uint line, uint column)
            {
                if (atomType != null)
                    return new ArgumentException(string.Format("Expected {0} at line: {1}, column: {2}", atomType, line, column));
                else
                    throw new ArgumentNullException("atomType");
            }
        }

        static protected Exception Validate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, int startIndex)
        {
            if (atoms == null)
                return new ArgumentNullException("atoms");
            else
                if (startIndex < 0 && atoms.Count <= startIndex)
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

        protected Product(ProductType productType, bool isTerminal)
        {
            _productType = productType;
            _isTerminal = isTerminal;
        }

        public ProductType ProductType
        {
            get
            {
                return _productType;
            }
        }

        public bool IsTerminal
        {
            get
            {
                return _isTerminal;
            }
        }

        private readonly bool _isTerminal;
        private readonly ProductType _productType;
    }
}
