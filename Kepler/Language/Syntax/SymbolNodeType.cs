﻿namespace Andrei15193.Kepler.Language.Syntax
{
    public enum SymbolNodeType
    {
        Invalid = 0,

        Identifier,
        Constant,
        Stop,
        Skip,
        Assert,
        Operator,
        Finally,
        New,
        OpeningAngleBracket,
        OpeningRoundParenthesis,
        OpeningSquareParenthesis,
        ClosingAngleBracket,
        ClosingRoundParenthesis,
        ClosingSquareParenthesis,
        Predicate,
        Scope,
        While,
        Then,
        Try,
        Catch,
        Begin,
        Colon,
        Comma,
        Do,
        Else,
        End,
        Fact,
        QualifiedIdentifier,
        Array,
        Type,
        GenericParameters,
        BoundedArray,
        TypeInstance,
        VariableDeclaration,
        Expression,
        FunctionCall
    }
}
