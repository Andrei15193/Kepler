using System;
using System.Linq;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.TerminalSymbols
{
    public sealed class OperatorSymbol
        : TerminalSymbol
    {
        // refactor OperatorSymbol.TryCreate
        static public bool TryCreate(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language, out OperatorSymbol operatorSymbol)
        {
            try
            {
                operatorSymbol = new OperatorSymbol(atom, language);
            }
            catch
            {
                operatorSymbol = null;
            }

            return (operatorSymbol != null);
        }

        public OperatorSymbol(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language)
            : base(SymbolNodeType.Operator)
        {
            if (atom != null)
                if (language != null)
                {
                    switch (atom.Code)
                    {
                        case Lexicon.Plus:
                        case Lexicon.Minus:
                        case Lexicon.Star:
                        case Lexicon.Percentage:
                        case Lexicon.Slash:
                        case Lexicon.Backslash:
                            _isArithmetic = true;
                            break;
                        case Lexicon.LessThan:
                        case Lexicon.LessThanOrEqualTo:
                        case Lexicon.Equal:
                        case Lexicon.GreaterThanOrEqualTo:
                        case Lexicon.GreaterThan:
                            _isRelation = true;
                            break;
                        case Lexicon.Negation:
                        case Lexicon.And:
                        case Lexicon.Or:
                            _isBoolean = true;
                            break;
                        default:
                            throw ExceptionFactory.CreateExpectedAtom("operator", atom.Line, atom.Column);
                    }

                    _symbol = language.GetSymbol(atom.Code);
                    _atom = atom;
                    _priority = language.Operators.Values.First(op => op.Code == _atom.Code).Priority;
                }
                else
                    throw new ArgumentNullException("language");
            else
                throw new ArgumentNullException("atom");
        }

        public override string Symbol
        {
            get
            {
                return _symbol;
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

        public int Priority
        {
            get
            {
                return _priority;
            }
        }

        public bool IsArithmetic
        {
            get
            {
                return _isArithmetic;
            }
        }

        public bool IsBoolean
        {
            get
            {
                return _isBoolean;
            }
        }

        public bool IsRelation
        {
            get
            {
                return _isRelation;
            }
        }

        private readonly bool _isArithmetic = false;
        private readonly bool _isBoolean = false;
        private readonly bool _isRelation = false;
        private readonly int _priority;
        private readonly string _symbol;
        private readonly ScannedAtom<Lexicon> _atom;
    }
}
