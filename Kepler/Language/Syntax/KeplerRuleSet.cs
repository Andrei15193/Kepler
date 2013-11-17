using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    internal sealed class KeplerRuleSet
        : RuleSet<Lexicon>
    {
        public KeplerRuleSet(ILanguage<Lexicon> language = null)
            : base(language ?? Language<Lexicon>.Default, defaultRule: "program", ignoreRuleNameCase: true)
        {
            Add("name",
                RuleNode<Lexicon>.Atom(Lexicon.Identifier));

            Add("constant",
                RuleNode<Lexicon>.Atom(Lexicon.CharConstant));
            Add("constant",
                RuleNode<Lexicon>.Atom(Lexicon.FloatNumericConstant));
            Add("constant",
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant));
            Add("constant",
                RuleNode<Lexicon>.Atom(Lexicon.StringConstant));

            Add("qualifiedIdentifier",
                RuleNode<Lexicon>.RuleSequence("identifierSequence"),
                RuleNode<Lexicon>.Rule("name"));
            Add("identifierSequence",
                RuleNode<Lexicon>.Rule("name"),
                RuleNode<Lexicon>.Atom(Lexicon.Scope));

            Add("unaryPrefixedArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Plus));
            Add("unaryPrefixedArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Minus));

            Add("binaryArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Plus));
            Add("binaryArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Minus));
            Add("binaryArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Star));
            Add("binaryArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Percentage));
            Add("binaryArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Slash));
            Add("binaryArithmeticOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Backslash));

            Add("unaryPrefixedBooleanOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Negation));

            Add("binaryBooleanOperator",
                RuleNode<Lexicon>.Atom(Lexicon.And));
            Add("binaryBooleanOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Or));
            Add("binaryBooleanOperator",
                RuleNode<Lexicon>.Atom(Lexicon.Equal));

            Add("arithmeticRelation",
                RuleNode<Lexicon>.Atom(Lexicon.LessThan));
            Add("arithmeticRelation",
                RuleNode<Lexicon>.Atom(Lexicon.LessThanOrEqualTo));
            Add("arithmeticRelation",
                RuleNode<Lexicon>.Atom(Lexicon.Equal));
            Add("arithmeticRelation",
                RuleNode<Lexicon>.Atom(Lexicon.GreaterThanOrEqualTo));
            Add("arithmeticRelation",
                RuleNode<Lexicon>.Atom(Lexicon.GreaterThan));

            Add("program",
                RuleNode<Lexicon>.RuleSequence("predicateDeclaration"));

            Add("predicateDeclaration",
                RuleNode<Lexicon>.Rule("predicateDefinition"));
            Add("predicateDeclaration",
                RuleNode<Lexicon>.Rule("factDefinition"));

            Add("predicateDefinition",
                RuleNode<Lexicon>.Atom(Lexicon.Predicate),
                RuleNode<Lexicon>.Rule("name"),
                RuleNode<Lexicon>.Rule("body"));
            Add("predicateDefinition",
                RuleNode<Lexicon>.Atom(Lexicon.Predicate),
                RuleNode<Lexicon>.Rule("name"),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule("variableDeclaration"),
                RuleNode<Lexicon>.RuleSequence("prameterSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis),
                RuleNode<Lexicon>.Rule("body"));

            Add("factDefinition",
                RuleNode<Lexicon>.Atom(Lexicon.Fact),
                RuleNode<Lexicon>.Rule("name"),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add("factDefinition",
                RuleNode<Lexicon>.Atom(Lexicon.Fact),
                RuleNode<Lexicon>.Rule("name"),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule("factParameter"),
                RuleNode<Lexicon>.RuleSequence("factParameterSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));

            Add("prameterSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule("variableDeclaration"));
            Add("factParameter",
                RuleNode<Lexicon>.Rule("variableDeclaration"),
                RuleNode<Lexicon>.Rule("arithmeticRelation"),
                RuleNode<Lexicon>.Rule("constant"));
            Add("factParameterSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule("factParameter"));

            Add("type",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.RuleSequence("array"));
            Add("type",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Rule("genericParameters"),
                RuleNode<Lexicon>.RuleSequence("array"));

            Add("genericParameters",
                RuleNode<Lexicon>.Atom(Lexicon.LessThan),
                RuleNode<Lexicon>.Rule("type"),
                RuleNode<Lexicon>.RuleSequence("genericParametersSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.GreaterThan));
            Add("genericParametersSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule("type"));

            Add("array",
                RuleNode<Lexicon>.Atom(Lexicon.OpeningSquareParenthesis),
                RuleNode<Lexicon>.AtomSequence(Lexicon.Comma),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingSquareParenthesis));

            Add("body",
                RuleNode<Lexicon>.Rule("statement"));
            Add("body",
                RuleNode<Lexicon>.Atom(Lexicon.Begin),
                RuleNode<Lexicon>.RuleSequence("statement"),
                RuleNode<Lexicon>.Atom(Lexicon.End));

            Add("typeInstance",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.RuleSequence("boundedArray"));
            Add("typeInstance",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Rule("genericParameters"),
                RuleNode<Lexicon>.RuleSequence("boundedArray"));

            Add("boundedArray",
                RuleNode<Lexicon>.Atom(Lexicon.OpeningSquareParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant),
                RuleNode<Lexicon>.RuleSequence("boundedArraySequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingSquareParenthesis));
            Add("boundedArraySequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant));

            Add("variableDeclaration",
                RuleNode<Lexicon>.Rule("name"),
                RuleNode<Lexicon>.Atom(Lexicon.Colon),
                RuleNode<Lexicon>.Rule("type"));

            Add("statement",
                RuleNode<Lexicon>.Rule("whenStatement"));
            Add("statement",
                RuleNode<Lexicon>.Rule("whileStatement"));
            Add("statement",
                RuleNode<Lexicon>.Rule("tryCatchFinallyStatement"));
            Add("statement",
                RuleNode<Lexicon>.Rule("throwStatement"),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add("statement",
                RuleNode<Lexicon>.Rule("variableDeclarationStatement"),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add("statement",
                RuleNode<Lexicon>.Rule("variableAssignmentStatement"),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add("statement",
                RuleNode<Lexicon>.Rule("functionCall"),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add("statement",
                RuleNode<Lexicon>.Rule("exitStatement"),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));

            Add("whenStatement",
                RuleNode<Lexicon>.Atom(Lexicon.When),
                RuleNode<Lexicon>.Rule("booleanExpression"),
                RuleNode<Lexicon>.Atom(Lexicon.Then),
                RuleNode<Lexicon>.Rule("body"));
            Add("whenStatement",
                RuleNode<Lexicon>.Atom(Lexicon.When),
                RuleNode<Lexicon>.Rule("booleanExpression"),
                RuleNode<Lexicon>.Atom(Lexicon.Then),
                RuleNode<Lexicon>.Rule("body"),
                RuleNode<Lexicon>.Atom(Lexicon.Else),
                RuleNode<Lexicon>.Rule("body"));

            Add("whileStatement",
                RuleNode<Lexicon>.Atom(Lexicon.While),
                RuleNode<Lexicon>.Rule("booleanExpression"),
                RuleNode<Lexicon>.Atom(Lexicon.Do),
                RuleNode<Lexicon>.Rule("body"));

            Add("tryCatchFinallyStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Try),
                RuleNode<Lexicon>.Rule("body"),
                RuleNode<Lexicon>.Rule("catchStatement"));
            Add("tryCatchFinallyStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Try),
                RuleNode<Lexicon>.Rule("body"),
                RuleNode<Lexicon>.Rule("catchStatement"),
                RuleNode<Lexicon>.Rule("finallyStatement"));
            Add("tryCatchFinallyStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Try),
                RuleNode<Lexicon>.Rule("body"),
                RuleNode<Lexicon>.Rule("finallyStatement"));

            Add("catchStatement",
                RuleNode<Lexicon>.Rule("catchAllStatement"));
            Add("catchStatement",
                RuleNode<Lexicon>.Rule("catchBlockStatement"),
                RuleNode<Lexicon>.RuleSequence("catchBlockStatement"));
            Add("catchStatement",
                RuleNode<Lexicon>.Rule("catchBlockStatement"),
                RuleNode<Lexicon>.RuleSequence("catchBlockStatement"),
                RuleNode<Lexicon>.Rule("catchAllStatement"));

            Add("catchBlockStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Catch),
                RuleNode<Lexicon>.Rule("variableDeclaration"),
                RuleNode<Lexicon>.Rule("body"));

            Add("catchAllStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Catch),
                RuleNode<Lexicon>.Rule("body"));

            Add("finallyStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Finally),
                RuleNode<Lexicon>.Rule("body"));

            Add("throwStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Throw));
            Add("throwStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Throw),
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"));
            Add("throwStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Throw),
                RuleNode<Lexicon>.Rule("functionCall"));

            Add("variableDeclarationStatement",
                RuleNode<Lexicon>.Rule("variableDeclaration"),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Rule("expression"));
            Add("variableDeclarationStatement",
                RuleNode<Lexicon>.Rule("variableDeclaration"),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule("typeInstance"));

            Add("variableAssignmentStatement",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Rule("expression"));
            Add("variableAssignmentStatement",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule("typeInstance"));

            Add("functionCall",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));
            Add("functionCall",
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));
            Add("functionCall",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule("expression"),
                RuleNode<Lexicon>.RuleSequence("expressionSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));
            Add("functionCall",
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule("expression"),
                RuleNode<Lexicon>.RuleSequence("expressionSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add("expressionSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule("expression"));

            Add("exitStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Stop));
            Add("exitStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Skip));
            Add("exitStatement",
                RuleNode<Lexicon>.Atom(Lexicon.Assert),
                RuleNode<Lexicon>.Rule("booleanExpression"));

            Add("expression",
                RuleNode<Lexicon>.Rule("arithmeticExpression"));
            Add("expression",
                RuleNode<Lexicon>.Rule("booleanExpression"));
            Add("expression",
                RuleNode<Lexicon>.Rule("otherExpression"));

            Add("arithmeticExpression",
                RuleNode<Lexicon>.Rule("arithmeticOperand"));
            Add("arithmeticExpression",
                RuleNode<Lexicon>.Rule("unaryPrefixedArithmeticOperator"),
                RuleNode<Lexicon>.Rule("arithmeticOperand"));
            Add("arithmeticExpression",
                RuleNode<Lexicon>.Rule("arithmeticExpression"),
                RuleNode<Lexicon>.Rule("binaryArithmeticOperator"),
                RuleNode<Lexicon>.Rule("arithmeticExpression"));
            Add("arithmeticExpression",
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule("arithmeticExpression"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add("arithmeticOperand",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"));
            Add("arithmeticOperand",
                RuleNode<Lexicon>.Rule("functionCall"));
            Add("arithmeticOperand",
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant));
            Add("arithmeticOperand",
                RuleNode<Lexicon>.Atom(Lexicon.FloatNumericConstant));

            Add("booleanExpression",
                RuleNode<Lexicon>.Rule("booleanOperand"));
            Add("booleanExpression",
                RuleNode<Lexicon>.Rule("unaryPrefixedBooleanOperator"),
                RuleNode<Lexicon>.Rule("booleanOperand"));
            Add("booleanExpression",
                RuleNode<Lexicon>.Rule("booleanExpression"),
                RuleNode<Lexicon>.Rule("binaryBooleanOperator"),
                RuleNode<Lexicon>.Rule("booleanExpression"));
            Add("booleanExpression",
                RuleNode<Lexicon>.Rule("arithmeticExpression"),
                RuleNode<Lexicon>.Rule("arithmeticRelation"),
                RuleNode<Lexicon>.Rule("arithmeticExpression"));
            Add("booleanExpression",
                RuleNode<Lexicon>.Rule("otherExpression"),
                RuleNode<Lexicon>.Rule("arithmeticRelation"),
                RuleNode<Lexicon>.Rule("otherExpression"));
            Add("booleanExpression",
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule("booleanExpression"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add("booleanOperand",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"));
            Add("booleanOperand",
                RuleNode<Lexicon>.Rule("functionCall"));
            Add("booleanOperand",
                RuleNode<Lexicon>.Atom(Lexicon.True));
            Add("booleanOperand",
                RuleNode<Lexicon>.Atom(Lexicon.False));
            Add("booleanOperand",
                RuleNode<Lexicon>.Atom(Lexicon.Stop));
            Add("booleanOperand",
                RuleNode<Lexicon>.Atom(Lexicon.Skip));

            Add("otherExpression",
                RuleNode<Lexicon>.Rule("otherOperand"));
            Add("otherExpression",
                RuleNode<Lexicon>.Rule("otherOperand"),
                RuleNode<Lexicon>.Rule("binaryArithmeticOperator"),
                RuleNode<Lexicon>.Rule("otherOperand"));
            Add("otherExpression",
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule("otherOperand"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add("otherOperand",
                RuleNode<Lexicon>.Rule("constant"));
            Add("otherOperand",
                RuleNode<Lexicon>.Rule("functionCall"));
            Add("otherOperand",
                RuleNode<Lexicon>.Rule("qualifiedIdentifier"));
        }
    }
}
