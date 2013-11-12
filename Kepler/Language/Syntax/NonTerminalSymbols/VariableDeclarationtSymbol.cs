using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class VariableDeclarationtSymbol
        : NonTerminalSymbol
    {
        static public bool TryCreate(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, out VariableDeclarationtSymbol variableDeclarationtSymbol, int startIndex)
        {
            try
            {
                variableDeclarationtSymbol = new VariableDeclarationtSymbol(atoms, language, startIndex);
            }
            catch
            {
                variableDeclarationtSymbol = null;
            }

            return (variableDeclarationtSymbol != null);
        }

        public VariableDeclarationtSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.VariableDeclaration)
        {
            Exception exception = Validate(atoms, startIndex, 3);

            if (exception == null)
                if (language != null)
                {
                    List<Symbol> symbols = new List<Symbol>();

                    _variableName = new IdentifierSymbol(atoms[startIndex]);
                    symbols.Add(_variableName);
                    symbols.Add(new ColonSymbol(atoms[startIndex + 1], language));
                    _variableType = new TypeSymbol(atoms, language, startIndex + 2);
                    symbols.AddRange(_variableType.Symbols);
                    _symblos = new ReadOnlyCollection<Symbol>(symbols);
                }
                else
                    throw new ArgumentNullException("atoms");
            else
                throw exception;
        }

        public override IReadOnlyList<Symbol> Symbols
        {
            get
            {
                return _symblos;
            }
        }

        public override uint Line
        {
            get
            {
                return _variableName.Line;
            }
        }

        public override uint Column
        {
            get
            {
                return _variableName.Column;
            }
        }

        public IdentifierSymbol VariableName
        {
            get
            {
                return _variableName;
            }
        }

        public TypeSymbol VariableType
        {
            get
            {
                return _variableType;
            }
        }

        private readonly IdentifierSymbol _variableName;
        private readonly TypeSymbol _variableType;
        private readonly IReadOnlyList<Symbol> _symblos;
    }
}
