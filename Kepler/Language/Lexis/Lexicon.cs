using System.Text.RegularExpressions;
using Andrei15193.Kepler.Language.Lexis.Attributes;

namespace Andrei15193.Kepler.Language.Lexis
{
    public enum Lexicon
        : uint
    {
        Invalid = 0,

        [PatternAtom(@"^([_a-zA-Z][_a-zA-Z0-9]{0,249})$", RegexOptions.Compiled, AtomAttribute.PatternType.Identifier)]
        Identifier,

        [PatternAtom(@"^(0|[1-9]\d*)$", RegexOptions.Compiled, AtomAttribute.PatternType.Constant)]
        IntegerNumericConstant,
        [PatternAtom(@"^(0|[1-9]\d*\.\d*[1-9])$", RegexOptions.Compiled, AtomAttribute.PatternType.Constant)]
        FloatNumericConstant,
        [EnclosedAtom("\"", "\"", AtomAttribute.EnclosureType.Constant)]
        StringConstant,
        [EnclosedAtom("'", "'", AtomAttribute.EnclosureType.Constant, innerSequenceLength: 1)]
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
        [LiteralAtom("throw", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Throw,
        [LiteralAtom("try", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Try,
        [LiteralAtom("catch", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Catch,
        [LiteralAtom("finally", AtomAttribute.LiteralType.KeyWord, isReservedWord: true)]
        Finally,

        [Priority(1), LiteralAtom("::", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Scope,
        [Priority(2), LiteralAtom("*", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Star,
        [Priority(2), LiteralAtom("%", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Percentage,
        [Priority(2), LiteralAtom("/", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Slash,
        [Priority(2), LiteralAtom("\\", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Backslash,
        [Priority(3), LiteralAtom("+", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Plus,
        [Priority(3), LiteralAtom("-", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Minus,
        [Priority(4), LiteralAtom("<", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        LessThan,
        [Priority(4), LiteralAtom("<=", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        LessThanOrEqualTo,
        [Priority(4), LiteralAtom("=", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Equal,
        [Priority(4), LiteralAtom(">=", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        GreaterThanOrEqualTo,
        [Priority(4), LiteralAtom(">", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        GreaterThan,
        [Priority(5), LiteralAtom("!", AtomAttribute.LiteralType.Operator, isReservedWord: false)]
        Negation,
        [Priority(6), LiteralAtom("and", AtomAttribute.LiteralType.Operator, isReservedWord: true)]
        And,
        [Priority(7), LiteralAtom("or", AtomAttribute.LiteralType.Operator, isReservedWord: true)]
        Or,

        [LiteralAtom("(", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        OpeningRoundParenthesis,
        [LiteralAtom(")", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        ClosingRoundParenthesis,
        [LiteralAtom("[", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        OpeningSquareParenthesis,
        [LiteralAtom("]", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        ClosingSquareParenthesis,
        [LiteralAtom(".", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        Dot,
        [LiteralAtom(":", AtomAttribute.LiteralType.Separator, isReservedWord: false)]
        Colon,
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
        NewLine,
    }
}
