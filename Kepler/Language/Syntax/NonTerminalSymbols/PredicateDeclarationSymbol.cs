using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class PredicateDeclarationSymbol
        : NonTerminalSymbol
    {
        public PredicateDeclarationSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.PredicateDeclaration)
        {
            Exception exception = Validate(atoms, startIndex, 4);

            if (exception == null)
            {
                List<Symbol> symbols = new List<Symbol> { new PredicateSymbol(atoms[startIndex], language) };

                _name = new IdentifierSymbol(atoms[startIndex + 1]);
                symbols.Add(_name);
                if (atoms[startIndex + 2].Code == Lexicon.OpeningRoundParenthesis)
                {
                    _parameters = new PrediateParametersSymbol(atoms, language, startIndex + 2);
                    symbols.AddRange(_parameters.Symbols);
                }
                _body = new BodySymbol(atoms, language, symbols.Count);

                _symbols = new ReadOnlyCollection<Symbol>(symbols);
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

        public IdentifierSymbol Name
        {
            get
            {
                return _name;
            }
        }

        public BodySymbol Body
        {
            get
            {
                return _body;
            }
        }

        public IReadOnlyList<VariableDeclarationtSymbol> Parameters
        {
            get
            {
                return _parameters.Parameters;
            }
        }

        private readonly IdentifierSymbol _name;
        private readonly BodySymbol _body;
        private readonly PrediateParametersSymbol _parameters;
        private readonly IReadOnlyList<Symbol> _symbols;
    }
}
