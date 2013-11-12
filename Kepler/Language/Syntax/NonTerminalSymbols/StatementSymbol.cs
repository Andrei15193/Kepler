using System;
using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class StatementSymbol
        : NonTerminalSymbol
    {
        public StatementSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.Statement)
        {
            Exception exception = Validate(atoms, startIndex, 1);

            if (exception == null)
            {
                WhenStatementSymbol whenStatementSymbol;

                if (WhenStatementSymbol.TryCreate(atoms, language, out whenStatementSymbol, startIndex))
                    _statement = whenStatementSymbol;
                else
                {
                    WhileStatementSymbol whileStatementSymbol;

                    if (WhileStatementSymbol.TryCreate(atoms, language, out whileStatementSymbol, startIndex))
                        _statement = whileStatementSymbol;
                    else
                    {
                        TryCatchFinallySymbol tryCatchFinallySymbol;

                        if (TryCatchFinallySymbol.TryCreate(atoms, language, out tryCatchFinallySymbol, startIndex))
                            _statement = tryCatchFinallySymbol;
                        else
                        {
                            ThrowStatementSymbol throwStatementSymbol;

                            if (ThrowStatementSymbol.TryCreate(atoms, language, out throwStatementSymbol, startIndex))
                                _statement = throwStatementSymbol;
                            else
                            {
                                VariableDeclarationStatementSymbol variableDeclarationStatementSymbol;

                                if (VariableDeclarationStatementSymbol.TryCreate(atoms, language, out variableDeclarationStatementSymbol, startIndex))
                                    _statement = variableDeclarationStatementSymbol;
                                else
                                {
                                    VariableAssignmentSymbol variableAssignmentSymbol;

                                    if (VariableAssignmentSymbol.TryCreate(atoms, language, out variableAssignmentSymbol, startIndex))
                                        _statement = variableAssignmentSymbol;
                                    else
                                    {
                                        FunctionCallSymbol functionCallSymbol;

                                        if (FunctionCallSymbol.TryCreate(atoms, language, out functionCallSymbol, startIndex))
                                            _statement = functionCallSymbol;
                                        else
                                            _statement = new ExitStatement(atoms, language, startIndex);
                                    }
                                }
                            }

                            if (_statement.Symbols.Count - startIndex >= atoms.Count
                                || atoms[_statement.Symbols.Count].Code != Lexicon.Semicolon)
                                throw ExceptionFactory.CreateExpectedSymbol(";", _statement.Symbols[_statement.Symbols.Count - 1].Line, _statement.Symbols[_statement.Symbols.Count - 1].Column);
                        }
                    }
                }
            }
            else
                throw exception;
        }

        public override IReadOnlyList<Symbol> Symbols
        {
            get
            {
                return _statement.Symbols;
            }
        }

        public override uint Line
        {
            get
            {
                return _statement.Line;
            }
        }

        public override uint Column
        {
            get
            {
                return _statement.Column;
            }
        }

        public Symbol Statement
        {
            get
            {
                return _statement;
            }
        }

        private readonly NonTerminalSymbol _statement;
    }
}
