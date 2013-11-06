using System;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.TerminalSymbols
{
    public sealed class ConstantSymbol
        : TerminalSymbol
    {
        // refactor ConstantSymbol.TryCreate
        public static bool TryCreate(ScannedAtom<Lexicon> atom, out ConstantSymbol constantSymbol)
        {
            try
            {
                constantSymbol = new ConstantSymbol(atom);
            }
            catch
            {
                constantSymbol = null;
            }

            return (constantSymbol != null);
        }

        public ConstantSymbol(ScannedAtom<Lexicon> atom)
            : base(SymbolNodeType.Constant)
        {
            if (atom != null)
                if (atom.HasValue)
                {
                    _atom = atom;
                    switch (atom.Code)
                    {
                        case Lexicon.IntegerNumericConstant:
                            _isInteger = true;
                            break;
                        case Lexicon.FloatNumericConstant:
                            _isFloat = true;
                            break;
                        case Lexicon.StringConstant:
                            _isString = true;
                            break;
                        case Lexicon.CharConstant:
                            _isCharacter = true;
                            break;
                        case Lexicon.False:
                        case Lexicon.True:
                            _isBoolean = true;
                            break;
                        default:
                            throw ExceptionFactory.CreateExpectedAtom("constant", atom.Line, atom.Column);
                    }
                }
                else
                    throw ExceptionFactory.CreateExpectedAtom("constant", atom.Line, atom.Column);
            else
                throw new ArgumentNullException("atom");
        }

        public override string Symbol
        {
            get
            {
                return _atom.Value;
            }
        }

        public override uint Line
        {
            get
            {
                return _atom.Line;
            }
        }

        public override uint Column
        {
            get
            {
                return _atom.Column;
            }
        }

        public bool IsNumeric
        {
            get
            {
                return (_isInteger || _isFloat);
            }
        }

        public bool IsBoolean
        {
            get
            {
                return _isBoolean;
            }
        }

        public bool IsCharacter
        {
            get
            {
                return _isCharacter;
            }
        }

        public bool IsInteger
        {
            get
            {
                return _isInteger;
            }
        }

        public bool IsFloat
        {
            get
            {
                return _isFloat;
            }
        }

        public bool IsString
        {
            get
            {
                return _isString;
            }
        }

        private readonly ScannedAtom<Lexicon> _atom;
        private readonly bool _isBoolean = false;
        private readonly bool _isCharacter = false;
        private readonly bool _isInteger = false;
        private readonly bool _isFloat = false;
        private readonly bool _isString = false;
    }
}
