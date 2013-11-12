using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Extensions;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class FuncitonParametersSymbol
        : NonTerminalSymbol
    {
        public FuncitonParametersSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.FuncitonParameters)
        {
            Exception exception = Validate(atoms, startIndex, 2);

            if (exception == null)
            {
                OpeningRoundParenthesisSymbol openingRoundParenthesisSymbol;
                ClosingRoundParenthesisSymbol closingRoundParenthesisSymbol = null;
                IList<ExpressionSymbol> parameters = new List<ExpressionSymbol>();

                if (OpeningRoundParenthesisSymbol.TryCreate(atoms[startIndex], language, out openingRoundParenthesisSymbol))
                {
                    List<Symbol> symbols = new List<Symbol>() { openingRoundParenthesisSymbol };
                    int openParenthesisCount = 1, parameterExpressionStartIndex = startIndex + 1, currentIndex = parameterExpressionStartIndex;

                    while (currentIndex < atoms.Count
                           && openParenthesisCount > 0
                           && !ClosingRoundParenthesisSymbol.TryCreate(atoms[currentIndex], language, out closingRoundParenthesisSymbol))
                    {
                        CommaSymbol commaSymbol;

                        if (ClosingRoundParenthesisSymbol.TryCreate(atoms[currentIndex], language, out closingRoundParenthesisSymbol))
                            openParenthesisCount--;
                        else
                            if (OpeningRoundParenthesisSymbol.TryCreate(atoms[currentIndex], language, out openingRoundParenthesisSymbol))
                                openParenthesisCount++;
                            else
                                if (openParenthesisCount == 1 && CommaSymbol.TryCreate(atoms[currentIndex], language, out commaSymbol))
                                {
                                    parameters.Add(new ExpressionSymbol(atoms.Sublist(parameterExpressionStartIndex, currentIndex - parameterExpressionStartIndex), language));
                                    symbols.Add(commaSymbol);
                                    symbols.AddRange(parameters[parameters.Count - 1].Symbols);
                                    parameterExpressionStartIndex = currentIndex + 1;
                                }
                        currentIndex++;
                    }
                    parameters.Add(new ExpressionSymbol(atoms.Sublist(parameterExpressionStartIndex, currentIndex - parameterExpressionStartIndex), language));
                    symbols.AddRange(parameters[parameters.Count - 1].Symbols);
                    symbols.Add(closingRoundParenthesisSymbol);

                    _parameters = new ReadOnlyCollection<ExpressionSymbol>(parameters);
                    _symbols = new ReadOnlyCollection<Symbol>(symbols);
                }
                else
                    throw ExceptionFactory.CreateExpectedSymbol("(", atoms[startIndex].Line, atoms[startIndex].Column);
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
                return _symbols[0].Line;
            }
        }

        public IReadOnlyList<ExpressionSymbol> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        private readonly IReadOnlyList<Symbol> _symbols;
        private readonly IReadOnlyList<ExpressionSymbol> _parameters;
    }
}
