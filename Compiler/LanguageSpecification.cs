namespace Andrei15193.Kepler.Compiler
{
    using Andrei15193.Kepler.Compiler.Attributes;
    using Andrei15193.Kepler.Compiler.Regex.Attributes;
    using System.Text.RegularExpressions;

    public enum LanguageSpecification : uint
    {
        Invalid = 0,

        [PatternAtom(@"^([_a-zA-Z][_a-zA-Z0-9]*)$", RegexOptions.Compiled, AtomAttribute.PatternType.Identifier)]
        Identifier,

        [PatternAtom(@"^(0|[1-9]\d*)$", RegexOptions.Compiled, AtomAttribute.PatternType.Constant)]
        NumericConstant,
        [EnclosedAtom("\"", "\"", AtomAttribute.EnclosureType.Constant)]
        StringConstant,
        [EnclosedAtom("'", "'", AtomAttribute.EnclosureType.Constant)]
        CharConstant,

        [PatternAtom(@"\#.*\n", RegexOptions.Compiled, AtomAttribute.PatternType.Ignore)]
        Comment,
        [EnclosedAtom("#{", "}", AtomAttribute.EnclosureType.Comment)]
        MultilineComment,

        [LiteralAtom("assert", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Assert,
        [LiteralAtom("begin", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Begin,
        [LiteralAtom("do", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Do,
        [LiteralAtom("else", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Else,
        [LiteralAtom("end", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        End,
        [LiteralAtom("fact", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Fact,
        [LiteralAtom("false", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        False,
        [LiteralAtom("new", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        New,
        [LiteralAtom("predicate", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Predicate,
        [LiteralAtom("skip", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Skip,
        [LiteralAtom("stop", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Stop,
        [LiteralAtom("then", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Then,
        [LiteralAtom("true", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        True,
        [LiteralAtom("when", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        When,
        [LiteralAtom("while", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        While,
        [LiteralAtom("try", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Try,
        [LiteralAtom("catch", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Catch,
        [LiteralAtom("finally", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Finally,

        [LiteralAtom("+", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Plus,
        [LiteralAtom("-", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Minus,
        [LiteralAtom("*", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Star,
        [LiteralAtom("/", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Slash,
        [LiteralAtom("\\", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Backslash,
        [LiteralAtom("<=", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        LessThanOrEqualTo,
        [LiteralAtom("=", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Equal,
        [LiteralAtom(">=", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        GreaterThanOrEqualTo,
        [LiteralAtom("and", AtomAttribute.LiteralType.Operator, isReservedWord: true)]
        And,
        [LiteralAtom("or", AtomAttribute.LiteralType.Operator, isReservedWord: true)]
        Or,
        [LiteralAtom("!", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Negation,
        [LiteralAtom(":", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Declaration,
        [LiteralAtom("::", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Scope,

        [LiteralAtom("(", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        OpeningParenthesis,
        [LiteralAtom(")", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        ClosingParenthesis,
        [LiteralAtom("<", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        LessThan,
        [LiteralAtom(">", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        GreaterThan,
        [LiteralAtom(".", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        Dot,
        [LiteralAtom(",", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        Comma,
        [LiteralAtom(" ", AtomAttribute.LiteralType.Separator, isReservedWord: false, consider: false)]
        WhiteSpace,
        [LiteralAtom("\t", AtomAttribute.LiteralType.Separator, isReservedWord: false, consider: false)]
        Tabulator,
        [LiteralAtom("\n", AtomAttribute.LiteralType.Separator, isReservedWord: false, consider: false)]
        LineFeed,
        [LiteralAtom("\r", AtomAttribute.LiteralType.Separator, isReservedWord: false, consider: false)]
        CarriageReturn,
        [LiteralAtom("\r\n", AtomAttribute.LiteralType.Separator, isReservedWord: false, consider: false)]
        NewLine
    }
}
