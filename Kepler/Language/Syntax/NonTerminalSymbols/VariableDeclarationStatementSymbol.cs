using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class VariableDeclarationStatementSymbol
        : NonTerminalSymbol
    {
        // refactor VariableDeclarationStatementSymbol.TryCreate
        static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out VariableDeclarationStatementSymbol variableDeclarationStatementSymbol, int startIndex)
        {
            try
            {
                variableDeclarationStatementSymbol = new VariableDeclarationStatementSymbol(atoms, language, startIndex);
            }
            catch
            {
                variableDeclarationStatementSymbol = null;
            }

            return (variableDeclarationStatementSymbol != null);
        }

        public VariableDeclarationStatementSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.VariableDeclarationStatement)
        {
            Exception exception = Validate(atoms, startIndex, 3);

            if (exception == null)
            {
                List<Symbol> symbols = new List<Symbol>();

                _variableDeclaration = new VariableDeclarationtSymbol(atoms, language, startIndex);
                symbols.AddRange(_variableDeclaration.Symbols);
                OperatorSymbol operatorSymbol = new OperatorSymbol(atoms[_variableDeclaration.Symbols.Count], language);
                if (operatorSymbol.Symbol != "=")
                    throw ExceptionFactory.CreateExpectedSymbol("=", operatorSymbol.Line, operatorSymbol.Column);
                else
                {
                    TypeInstanceSymbol typeInstanceSymbol;

                    symbols.Add(operatorSymbol);
                    if (TypeInstanceSymbol.TryCreate(atoms, language, out typeInstanceSymbol, symbols.Count))
                        _value = typeInstanceSymbol;
                    else
                        _value = new ExpressionSymbol(atoms, language, symbols.Count);
                    symbols.AddRange(_value.Symbols);

                    _symbols = new ReadOnlyCollection<Symbol>(symbols);
                }
            }
            else
                throw exception;
        }

        public override IReadOnlyList<Symbol> Symbols
        {
            get
            {
                return _symbols;
            }
        }

        public override uint Line
        {
            get
            {
                return _symbols[0].Line;
            }
        }

        public override uint Column
        {
            get
            {
                return _symbols[0].Column;
            }
        }

        public VariableDeclarationtSymbol VariableName
        {
            get
            {
                return _variableDeclaration;
            }
        }

        public Symbol Value
        {
            get
            {
                return _value;
            }
        }

        private readonly NonTerminalSymbol _value;
        private readonly VariableDeclarationtSymbol _variableDeclaration;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
