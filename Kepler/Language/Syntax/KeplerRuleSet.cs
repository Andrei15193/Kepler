using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class KeplerRuleSet
        : RuleSet<Lexicon>
    {
        public const string Name = "name";
        public const string Constant = "constant";
        public const string QualifiedIdentifier = "qualifiedIdentifier";
        public const string UnaryPrefixedArithmeticOperator = "unaryPrefixedArithmeticOperator";
        public const string BinaryArithmeticOperator = "binaryArithmeticOperator";
        public const string UnaryPrefixedBooleanOperator = "unaryPrefixedBooleanOperator";
        public const string BinaryBooleanOperator = "binaryBooleanOperator";
        public const string ArithmeticRelation = "arithmeticRelation";
        public const string Program = "program";
        public const string PredicateDeclaration = "predicateDeclaration";
        public const string PredicateDefinition = "predicateDefinition";
        public const string FactDefinition = "factDefinition";
        public const string FactParameter = "factParameter";
        public const string Type = "type";
        public const string GenericParameters = "genericParameters";
        public const string Array = "array";
        public const string Body = "body";
        public const string TypeInstance = "typeInstance";
        public const string BoundedArray = "boundedArray";
        public const string VariableDeclaration = "variableDeclaration";
        public const string Statement = "statement";
        public const string WhenStatement = "whenStatement";
        public const string WhileStatement = "whileStatement";
        public const string TryCatchFinallyStatement = "tryCatchFinallyStatement";
        public const string CatchStatement = "catchStatement";
        public const string CatchBlockStatement = "catchBlockStatement";
        public const string CatchAllStatement = "catchAllStatement";
        public const string FinallyStatement = "finallyStatement";
        public const string ThrowStatement = "throwStatement";
        public const string VariableDeclarationStatement = "variableDeclarationStatement";
        public const string VariableAssignmentStatement = "variableAssignmentStatement";
        public const string FunctionCall = "functionCall";
        public const string ExitStatement = "exitStatement";
        public const string Expression = "expression";
        public const string ArithmeticExpression = "arithmeticExpression";
        public const string ArithmeticOperand = "arithmeticOperand";
        public const string BooleanExpression = "booleanExpression";
        public const string BooleanOperand = "booleanOperand";
        public const string OtherExpression = "otherExpression";
        public const string OtherOperand = "otherOperand";

        public KeplerRuleSet(ILanguage<Lexicon> language = null)
            : base(language ?? Language<Lexicon>.Default, defaultRule: "program", ignoreRuleNameCase: true)
        {
            Add(Name,
                RuleNode<Lexicon>.Atom(Lexicon.Identifier));

            Add(Constant,
                RuleNode<Lexicon>.Atom(Lexicon.CharConstant));
            Add(Constant,
                RuleNode<Lexicon>.Atom(Lexicon.FloatNumericConstant));
            Add(Constant,
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant));
            Add(Constant,
                RuleNode<Lexicon>.Atom(Lexicon.StringConstant));

            Add(QualifiedIdentifier,
                RuleNode<Lexicon>.RuleSequence("identifierSequence"),
                RuleNode<Lexicon>.Rule(Name));
            Add("identifierSequence",
                RuleNode<Lexicon>.Rule(Name),
                RuleNode<Lexicon>.Atom(Lexicon.Scope));

            Add(UnaryPrefixedArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Plus));
            Add(UnaryPrefixedArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Minus));

            Add(BinaryArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Plus));
            Add(BinaryArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Minus));
            Add(BinaryArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Star));
            Add(BinaryArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Percentage));
            Add(BinaryArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Slash));
            Add(BinaryArithmeticOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Backslash));

            Add(UnaryPrefixedBooleanOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Negation));

            Add(BinaryBooleanOperator,
                RuleNode<Lexicon>.Atom(Lexicon.And));
            Add(BinaryBooleanOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Or));
            Add(BinaryBooleanOperator,
                RuleNode<Lexicon>.Atom(Lexicon.Equal));

            Add(ArithmeticRelation,
                RuleNode<Lexicon>.Atom(Lexicon.LessThan));
            Add(ArithmeticRelation,
                RuleNode<Lexicon>.Atom(Lexicon.LessThanOrEqualTo));
            Add(ArithmeticRelation,
                RuleNode<Lexicon>.Atom(Lexicon.Equal));
            Add(ArithmeticRelation,
                RuleNode<Lexicon>.Atom(Lexicon.GreaterThanOrEqualTo));
            Add(ArithmeticRelation,
                RuleNode<Lexicon>.Atom(Lexicon.GreaterThan));

            Add(Program,
                RuleNode<Lexicon>.RuleSequence(PredicateDeclaration));

            Add(PredicateDeclaration,
                RuleNode<Lexicon>.Rule(PredicateDefinition));
            Add(PredicateDeclaration,
                RuleNode<Lexicon>.Rule(FactDefinition));

            Add(PredicateDefinition,
                RuleNode<Lexicon>.Atom(Lexicon.Predicate),
                RuleNode<Lexicon>.Rule(Name),
                RuleNode<Lexicon>.Rule(Body));
            Add(PredicateDefinition,
                RuleNode<Lexicon>.Atom(Lexicon.Predicate),
                RuleNode<Lexicon>.Rule(Name),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule(VariableDeclaration),
                RuleNode<Lexicon>.RuleSequence("parameterSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis),
                RuleNode<Lexicon>.Rule(Body));

            Add(FactDefinition,
                RuleNode<Lexicon>.Atom(Lexicon.Fact),
                RuleNode<Lexicon>.Rule(Name),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add(FactDefinition,
                RuleNode<Lexicon>.Atom(Lexicon.Fact),
                RuleNode<Lexicon>.Rule(Name),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule(FactParameter),
                RuleNode<Lexicon>.RuleSequence("factParameterSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));

            Add("parameterSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule(VariableDeclaration));
            Add(FactParameter,
                RuleNode<Lexicon>.Rule(VariableDeclaration),
                RuleNode<Lexicon>.Rule(ArithmeticRelation),
                RuleNode<Lexicon>.Rule(Constant));
            Add("factParameterSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule(FactParameter));

            Add(Type,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.RuleSequence("ArraySequence"));
            Add(Type,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Rule(GenericParameters),
                RuleNode<Lexicon>.RuleSequence("ArraySequence"));

            Add(GenericParameters,
                RuleNode<Lexicon>.Atom(Lexicon.LessThan),
                RuleNode<Lexicon>.Rule(Type),
                RuleNode<Lexicon>.RuleSequence("genericParametersSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.GreaterThan));
            Add("genericParametersSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule(Type));

            Add("ArraySequence",
                RuleNode<Lexicon>.Rule(KeplerRuleSet.Array));
            Add(Array,
                RuleNode<Lexicon>.Atom(Lexicon.OpeningSquareParenthesis),
                RuleNode<Lexicon>.AtomSequence(Lexicon.Comma),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingSquareParenthesis));

            Add(Body,
                RuleNode<Lexicon>.Rule(Statement));
            Add(Body,
                RuleNode<Lexicon>.Atom(Lexicon.Begin),
                RuleNode<Lexicon>.RuleSequence(Statement),
                RuleNode<Lexicon>.Atom(Lexicon.End));

            Add(TypeInstance,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.RuleSequence(BoundedArray));
            Add(TypeInstance,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Rule(GenericParameters),
                RuleNode<Lexicon>.RuleSequence(BoundedArray));

            Add(BoundedArray,
                RuleNode<Lexicon>.Atom(Lexicon.OpeningSquareParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant),
                RuleNode<Lexicon>.RuleSequence("boundedArraySequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingSquareParenthesis));
            Add("boundedArraySequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant));

            Add(VariableDeclaration,
                RuleNode<Lexicon>.Rule(Name),
                RuleNode<Lexicon>.Atom(Lexicon.Colon),
                RuleNode<Lexicon>.Rule(Type));

            Add(Statement,
                RuleNode<Lexicon>.Rule(WhenStatement));
            Add(Statement,
                RuleNode<Lexicon>.Rule(WhileStatement));
            Add(Statement,
                RuleNode<Lexicon>.Rule(TryCatchFinallyStatement));
            Add(Statement,
                RuleNode<Lexicon>.Rule(ThrowStatement),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add(Statement,
                RuleNode<Lexicon>.Rule(VariableDeclarationStatement),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add(Statement,
                RuleNode<Lexicon>.Rule(VariableAssignmentStatement),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add(Statement,
                RuleNode<Lexicon>.Rule(FunctionCall),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));
            Add(Statement,
                RuleNode<Lexicon>.Rule(ExitStatement),
                RuleNode<Lexicon>.Atom(Lexicon.Dot));

            Add(WhenStatement,
                RuleNode<Lexicon>.Atom(Lexicon.When),
                RuleNode<Lexicon>.Rule(BooleanExpression),
                RuleNode<Lexicon>.Atom(Lexicon.Then),
                RuleNode<Lexicon>.Rule(Body));
            Add(WhenStatement,
                RuleNode<Lexicon>.Atom(Lexicon.When),
                RuleNode<Lexicon>.Rule(BooleanExpression),
                RuleNode<Lexicon>.Atom(Lexicon.Then),
                RuleNode<Lexicon>.Rule(Body),
                RuleNode<Lexicon>.Atom(Lexicon.Else),
                RuleNode<Lexicon>.Rule(Body));

            Add(WhileStatement,
                RuleNode<Lexicon>.Atom(Lexicon.While),
                RuleNode<Lexicon>.Rule(BooleanExpression),
                RuleNode<Lexicon>.Atom(Lexicon.Do),
                RuleNode<Lexicon>.Rule(Body));

            Add(TryCatchFinallyStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Try),
                RuleNode<Lexicon>.Rule(Body),
                RuleNode<Lexicon>.Rule(CatchStatement));
            Add(TryCatchFinallyStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Try),
                RuleNode<Lexicon>.Rule(Body),
                RuleNode<Lexicon>.Rule(CatchStatement),
                RuleNode<Lexicon>.Rule(FinallyStatement));
            Add(TryCatchFinallyStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Try),
                RuleNode<Lexicon>.Rule(Body),
                RuleNode<Lexicon>.Rule(FinallyStatement));

            Add(CatchStatement,
                RuleNode<Lexicon>.Rule(CatchAllStatement));
            Add(CatchStatement,
                RuleNode<Lexicon>.Rule(CatchBlockStatement),
                RuleNode<Lexicon>.RuleSequence(CatchBlockStatement));
            Add(CatchStatement,
                RuleNode<Lexicon>.Rule(CatchBlockStatement),
                RuleNode<Lexicon>.RuleSequence(CatchBlockStatement),
                RuleNode<Lexicon>.Rule(CatchAllStatement));

            Add(CatchBlockStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Catch),
                RuleNode<Lexicon>.Rule(VariableDeclaration),
                RuleNode<Lexicon>.Rule(Body));

            Add(CatchAllStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Catch),
                RuleNode<Lexicon>.Rule(Body));

            Add(FinallyStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Finally),
                RuleNode<Lexicon>.Rule(Body));

            Add(ThrowStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Throw));
            Add(ThrowStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Throw),
                RuleNode<Lexicon>.Rule(QualifiedIdentifier));
            Add(ThrowStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Throw),
                RuleNode<Lexicon>.Rule(FunctionCall));

            Add(VariableDeclarationStatement,
                RuleNode<Lexicon>.Rule(VariableDeclaration),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Rule(Expression));
            Add(VariableDeclarationStatement,
                RuleNode<Lexicon>.Rule(VariableDeclaration),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule(TypeInstance));

            Add(VariableAssignmentStatement,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Rule(Expression));
            Add(VariableAssignmentStatement,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Atom(Lexicon.Equal),
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule(TypeInstance));

            Add(FunctionCall,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));
            Add(FunctionCall,
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));
            Add(FunctionCall,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule(Expression),
                RuleNode<Lexicon>.RuleSequence("expressionSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));
            Add(FunctionCall,
                RuleNode<Lexicon>.Atom(Lexicon.New),
                RuleNode<Lexicon>.Rule(QualifiedIdentifier),
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule(Expression),
                RuleNode<Lexicon>.RuleSequence("expressionSequence"),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add("expressionSequence",
                RuleNode<Lexicon>.Atom(Lexicon.Comma),
                RuleNode<Lexicon>.Rule(Expression));

            Add(ExitStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Stop));
            Add(ExitStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Skip));
            Add(ExitStatement,
                RuleNode<Lexicon>.Atom(Lexicon.Assert),
                RuleNode<Lexicon>.Rule(BooleanExpression));

            Add(Expression,
                RuleNode<Lexicon>.Rule(ArithmeticExpression));
            Add(Expression,
                RuleNode<Lexicon>.Rule(BooleanExpression));
            Add(Expression,
                RuleNode<Lexicon>.Rule(OtherExpression));

            Add(ArithmeticExpression,
                RuleNode<Lexicon>.Rule(ArithmeticOperand));
            Add(ArithmeticExpression,
                RuleNode<Lexicon>.Rule(UnaryPrefixedArithmeticOperator),
                RuleNode<Lexicon>.Rule(ArithmeticOperand));
            Add(ArithmeticExpression,
                RuleNode<Lexicon>.Rule(ArithmeticExpression),
                RuleNode<Lexicon>.Rule(BinaryArithmeticOperator),
                RuleNode<Lexicon>.Rule(ArithmeticExpression));
            Add(ArithmeticExpression,
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule(ArithmeticExpression),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add(ArithmeticOperand,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier));
            Add(ArithmeticOperand,
                RuleNode<Lexicon>.Rule(FunctionCall));
            Add(ArithmeticOperand,
                RuleNode<Lexicon>.Atom(Lexicon.IntegerNumericConstant));
            Add(ArithmeticOperand,
                RuleNode<Lexicon>.Atom(Lexicon.FloatNumericConstant));

            Add(BooleanExpression,
                RuleNode<Lexicon>.Rule(BooleanOperand));
            Add(BooleanExpression,
                RuleNode<Lexicon>.Rule(UnaryPrefixedBooleanOperator),
                RuleNode<Lexicon>.Rule(BooleanOperand));
            Add(BooleanExpression,
                RuleNode<Lexicon>.Rule(BooleanExpression),
                RuleNode<Lexicon>.Rule(BinaryBooleanOperator),
                RuleNode<Lexicon>.Rule(BooleanExpression));
            Add(BooleanExpression,
                RuleNode<Lexicon>.Rule(ArithmeticExpression),
                RuleNode<Lexicon>.Rule(ArithmeticRelation),
                RuleNode<Lexicon>.Rule(ArithmeticExpression));
            Add(BooleanExpression,
                RuleNode<Lexicon>.Rule(OtherExpression),
                RuleNode<Lexicon>.Rule(ArithmeticRelation),
                RuleNode<Lexicon>.Rule(OtherExpression));
            Add(BooleanExpression,
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule(BooleanExpression),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add(BooleanOperand,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier));
            Add(BooleanOperand,
                RuleNode<Lexicon>.Rule(FunctionCall));
            Add(BooleanOperand,
                RuleNode<Lexicon>.Atom(Lexicon.True));
            Add(BooleanOperand,
                RuleNode<Lexicon>.Atom(Lexicon.False));
            Add(BooleanOperand,
                RuleNode<Lexicon>.Atom(Lexicon.Stop));
            Add(BooleanOperand,
                RuleNode<Lexicon>.Atom(Lexicon.Skip));

            Add(OtherExpression,
                RuleNode<Lexicon>.Rule(OtherOperand));
            Add(OtherExpression,
                RuleNode<Lexicon>.Rule(OtherOperand),
                RuleNode<Lexicon>.Rule(BinaryArithmeticOperator),
                RuleNode<Lexicon>.Rule(OtherOperand));
            Add(OtherExpression,
                RuleNode<Lexicon>.Atom(Lexicon.OpeningRoundParenthesis),
                RuleNode<Lexicon>.Rule(OtherOperand),
                RuleNode<Lexicon>.Atom(Lexicon.ClosingRoundParenthesis));

            Add(OtherOperand,
                RuleNode<Lexicon>.Rule(Constant));
            Add(OtherOperand,
                RuleNode<Lexicon>.Rule(FunctionCall));
            Add(OtherOperand,
                RuleNode<Lexicon>.Rule(QualifiedIdentifier));
        }
    }
}
