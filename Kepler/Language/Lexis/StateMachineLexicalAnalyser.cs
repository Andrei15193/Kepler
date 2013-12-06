using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.AbstractCore;

namespace Andrei15193.Kepler.Language.Lexis
{
    public sealed class StateMachineLexicalAnalyser
        : ILexicalAnalyser<Lexicon>
    {
        static StateMachineLexicalAnalyser()
        {
            _atomRecognizers.Add(_CreateCommentAtomRecognizer());
            _atomRecognizers.Add(_CreateMultilineCommentAtomRecognizer());
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("assert", Lexicon.Assert, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("begin", Lexicon.Begin, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("do", Lexicon.Do, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("else", Lexicon.Else, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("end", Lexicon.End, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("fact", Lexicon.Fact, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("false", Lexicon.False, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("new", Lexicon.New, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("predicate", Lexicon.Predicate, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("skip", Lexicon.Skip, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("stop", Lexicon.Stop, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("then", Lexicon.Then, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("true", Lexicon.True, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("when", Lexicon.When, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("while", Lexicon.While, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("throw", Lexicon.Throw, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("try", Lexicon.Try, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("catch", Lexicon.Catch, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("finally", Lexicon.Finally, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("::", Lexicon.Scope, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("*", Lexicon.Star, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("%", Lexicon.Percentage, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("/", Lexicon.Slash, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\\", Lexicon.Backslash, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("+", Lexicon.Plus, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("-", Lexicon.Minus, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("<", Lexicon.LessThan, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("<=", Lexicon.LessThanOrEqualTo, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("=", Lexicon.Equal, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(">=", Lexicon.GreaterThanOrEqualTo, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(">", Lexicon.GreaterThan, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("negation", Lexicon.Negation, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("and", Lexicon.And, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("or", Lexicon.Or, isDelimiter: false));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("(", Lexicon.OpeningRoundParenthesis, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(")", Lexicon.ClosingRoundParenthesis, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("[", Lexicon.OpeningSquareParenthesis, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("]", Lexicon.ClosingSquareParenthesis, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(":", Lexicon.Colon, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(",", Lexicon.Comma, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(".", Lexicon.Dot, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(" ", Lexicon.WhiteSpace, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\t", Lexicon.Tabulator, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\n", Lexicon.LineFeed, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\r", Lexicon.CarriageReturn, isDelimiter: true));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\r\n", Lexicon.NewLine, isDelimiter: true));
            _atomRecognizers.Add(_CreateIdentifierAtomRecognizer());
            _atomRecognizers.Add(_CreateIntegerConstantAtomRecognizer());
            _atomRecognizers.Add(_CreateFloatConstantAtomRecognizer());
            _atomRecognizers.Add(_CreateCharConstantAtomRecognizer());
            _atomRecognizers.Add(_CreateStringConstantAtomRecognizer());
        }

        private static AtomRecognizer<Lexicon> _CreateCommentAtomRecognizer()
        {
            State<char> startState = new State<char>("start", isFinalState: false);
            State<char> commentStart = new State<char>("commentStart",
                                                       isFinalState: false,
                                                       transitionToSelfCondition: character => character != '\r');
            State<char> carriageReturn = new State<char>("carriageReturn", isFinalState: false);
            State<char> lineFeed = new State<char>("lineFeed", isFinalState: true);

            commentStart.Transitions.Add(new Transition<char>(character => character == '\r', carriageReturn));
            carriageReturn.Transitions.Add(new Transition<char>(character => character == '\n', lineFeed));

            return new AtomRecognizer<Lexicon>(Lexicon.Comment, startState, isDelimiter: true);
        }

        private static AtomRecognizer<Lexicon> _CreateMultilineCommentAtomRecognizer()
        {
            State<char> startState = new State<char>("start", isFinalState: false);
            State<char> sharp = new State<char>("sharp", isFinalState: false);
            State<char> commentStart = new State<char>("commentStart",
                                                       isFinalState: false,
                                                       transitionToSelfCondition: character => character != '}');
            State<char> commentEnd = new State<char>("}", isFinalState: true);

            startState.Transitions.Add(new Transition<char>(character => character == '#', sharp));
            sharp.Transitions.Add(new Transition<char>(character => character == '{', commentStart));
            commentStart.Transitions.Add(new Transition<char>(character => character == '}', commentEnd));

            return new AtomRecognizer<Lexicon>(Lexicon.MultilineComment, startState, isDelimiter: true);
        }

        private static AtomRecognizer<Lexicon> _CreateLiteralAtomRecognizer(string literal, Lexicon code, bool isDelimiter)
        {
            if (literal != null)
                if (literal.Length > 0)
                {
                    State<char> startState = new State<char>("start", isFinalState: false);
                    State<char> previousState = startState;

                    for (int literalCharIndex = 0; literalCharIndex < literal.Length - 1; literalCharIndex++)
                    {
                        State<char> newState = new State<char>(literal[literalCharIndex] + "Character", isFinalState: false);

                        previousState.Transitions.Add(new Transition<char>(_GetLambda(literal, literalCharIndex), newState));
                        previousState = newState;
                    }
                    previousState.Transitions.Add(new Transition<char>(character => character == literal[literal.Length - 1], new State<char>(literal[literal.Length - 1] + "Character", isFinalState: true)));

                    return new AtomRecognizer<Lexicon>(code, startState, isDelimiter);
                }
                else
                    throw new ArgumentException("Cannot be empty!", "literal");
            else
                throw new ArgumentNullException("literal");
        }

        private static Func<char, bool> _GetLambda(string literal, int literalCharIndex)
        {
            return (character => character == literal[literalCharIndex]);
        }

        private static AtomRecognizer<Lexicon> _CreateIdentifierAtomRecognizer()
        {
            State<char> startState = new State<char>("start", isFinalState: false);
            State<char> identifierSymbol = new State<char>("identifierSymbol",
                                                           isFinalState: true,
                                                           transitionToSelfCondition: (character => (character == '_'
                                                                                       || ('a' <= character && character <= 'z')
                                                                                       || ('A' <= character && character <= 'Z')
                                                                                       || char.IsDigit(character))));

            startState.Transitions.Add(new Transition<char>((character => (character == '_'
                                                                           || ('a' <= character && character <= 'z')
                                                                           || ('A' <= character && character <= 'Z'))),
                                                            identifierSymbol));

            return new AtomRecognizer<Lexicon>(Lexicon.Identifier, startState, isDelimiter: false);
        }

        private static AtomRecognizer<Lexicon> _CreateIntegerConstantAtomRecognizer()
        {
            State<char> startState = new State<char>("start", isFinalState: false);
            State<char> integerPartState = new State<char>("integerPart",
                                                           isFinalState: true,
                                                           transitionToSelfCondition: (character => char.IsDigit(character)));
            State<char> zeroState = new State<char>("zero", isFinalState: true);

            startState.Transitions.Add(new Transition<char>(character => (character != '0' && char.IsDigit(character)), integerPartState));
            startState.Transitions.Add(new Transition<char>(character => character == '0', zeroState));

            return new AtomRecognizer<Lexicon>(Lexicon.IntegerNumericConstant, startState, isDelimiter: false);
        }

        private static AtomRecognizer<Lexicon> _CreateFloatConstantAtomRecognizer()
        {
            State<char> startState = new State<char>("start", isFinalState: false);
            State<char> integerPartState = new State<char>("integerPart",
                                                           isFinalState: false,
                                                           transitionToSelfCondition: (character => char.IsDigit(character)));
            State<char> dotState = new State<char>("dot", isFinalState: false);
            State<char> realPartState = new State<char>("realPart",
                                                           isFinalState: true,
                                                           transitionToSelfCondition: (character => char.IsDigit(character)));
            State<char> zeroState = new State<char>("zero", isFinalState: true);

            startState.Transitions.Add(new Transition<char>(character => (character != '0' && char.IsDigit(character)), integerPartState));
            startState.Transitions.Add(new Transition<char>(character => character == '0', zeroState));
            integerPartState.Transitions.Add(new Transition<char>(character => character == '.', dotState));
            dotState.Transitions.Add(new Transition<char>(character => char.IsDigit(character), realPartState));

            return new AtomRecognizer<Lexicon>(Lexicon.FloatNumericConstant, startState, isDelimiter: false);
        }

        private static AtomRecognizer<Lexicon> _CreateCharConstantAtomRecognizer()
        {
            State<char> startState = new State<char>("start", isFinalState: false);
            State<char> openningApostrophe = new State<char>("openningApostrophe", isFinalState: false);
            State<char> nonEscapedCharacters = new State<char>("nonEscapedCharacters", isFinalState: false);
            State<char> backSlash = new State<char>("backSlash", isFinalState: false);
            State<char> escapedCharacters = new State<char>("escapedCharacters", isFinalState: false);
            State<char> closingApostrophe = new State<char>("closingApostrophe", isFinalState: true);

            startState.Transitions.Add(new Transition<char>(character => character == '\'', openningApostrophe));
            openningApostrophe.Transitions.Add(new Transition<char>(character => character != '\'' && character != '\\', nonEscapedCharacters));
            openningApostrophe.Transitions.Add(new Transition<char>(character => character == '\\', backSlash));
            backSlash.Transitions.Add(new Transition<char>(character => _escapedCharacters.Contains(character) || character == '\'', escapedCharacters));
            escapedCharacters.Transitions.Add(new Transition<char>(character => character == '\'', closingApostrophe));
            nonEscapedCharacters.Transitions.Add(new Transition<char>(character => character == '\'', closingApostrophe));

            return new AtomRecognizer<Lexicon>(Lexicon.CharConstant, startState, isDelimiter: false);
        }

        private static AtomRecognizer<Lexicon> _CreateStringConstantAtomRecognizer()
        {
            State<char> startState = new State<char>("start", isFinalState: false);
            State<char> openningQuote = new State<char>("openningQuote", isFinalState: false);
            State<char> nonEscapedCharacters = new State<char>("nonEscapedCharacters", isFinalState: false);
            State<char> backSlash = new State<char>("backSlash", isFinalState: false);
            State<char> escapedCharacters = new State<char>("escapedCharacters", isFinalState: false);
            State<char> closingQuote = new State<char>("closingQuote", isFinalState: true);

            startState.Transitions.Add(new Transition<char>(character => character == '"', openningQuote));
            openningQuote.Transitions.Add(new Transition<char>(character => character == '\\', backSlash));
            openningQuote.Transitions.Add(new Transition<char>(character => character == '"', closingQuote));
            openningQuote.Transitions.Add(new Transition<char>(character => character != '"' && character != '\\', nonEscapedCharacters));
            escapedCharacters.Transitions.Add(new Transition<char>(character => character == '"', closingQuote));
            escapedCharacters.Transitions.Add(new Transition<char>(character => character == '\\', backSlash));
            escapedCharacters.Transitions.Add(new Transition<char>(character => character != '"' && character != '\\', nonEscapedCharacters));
            nonEscapedCharacters.Transitions.Add(new Transition<char>(character => character == '"', closingQuote));
            nonEscapedCharacters.Transitions.Add(new Transition<char>(character => character == '\\', backSlash));
            nonEscapedCharacters.Transitions.Add(new Transition<char>(character => character != '"' && character != '\\', nonEscapedCharacters));
            backSlash.Transitions.Add(new Transition<char>(character => _escapedCharacters.Contains(character) || character == '"', escapedCharacters));

            return new AtomRecognizer<Lexicon>(Lexicon.StringConstant, startState, isDelimiter: false);
        }

        static private char[] _escapedCharacters = new[] { 'r', 'n', 't' };
        static private IList<AtomRecognizer<Lexicon>> _atomRecognizers = new List<AtomRecognizer<Lexicon>>();

        public ScanResult<Lexicon> Scan(string text, ILanguage<Lexicon> language)
        {
            if (text != null)
                if (language != null)
                {
                    bool wasPreviousDelimiter = true;
                    uint line = 1, column = 1;
                    int currentIndex = 0;
                    List<ScannedAtom<Lexicon>> scannedAtoms = new List<ScannedAtom<Lexicon>>();
                    IDictionary<string, string> identifiers = new SortedDictionary<string, string>();
                    IDictionary<string, string> constants = new SortedDictionary<string, string>();

                    while (currentIndex < text.Length)
                    {
                        AtomRecognitionResult<Lexicon> recognizedAtom = _RecognizeAtom(text, currentIndex, wasPreviousDelimiter);

                        if (recognizedAtom.Success)
                            if (wasPreviousDelimiter || recognizedAtom.IsDelimiter)
                            {
                                int indexOfLastNewLine = 0;
                                uint newLineCount = 0;

                                for (int indexOfNewLine = recognizedAtom.Sequence.IndexOf(Environment.NewLine); indexOfNewLine != -1; indexOfLastNewLine = indexOfNewLine, indexOfNewLine = recognizedAtom.Sequence.IndexOf(Environment.NewLine, indexOfNewLine + Environment.NewLine.Length))
                                    newLineCount++;
                                if (newLineCount > 0)
                                {
                                    line += newLineCount;
                                    column = (uint)(recognizedAtom.Sequence.Length - indexOfLastNewLine) - 1;
                                }
                                else
                                    column += (uint)recognizedAtom.Sequence.Length;

                                if (!language.CanIgnore(recognizedAtom.Code))
                                {
                                    string value;
                                    ScannedAtom<Lexicon> scannedAtom;

                                    switch (recognizedAtom.Code)
                                    {
                                        case Lexicon.Identifier:
                                            if (!identifiers.TryGetValue(recognizedAtom.Sequence, out value))
                                                identifiers.Add(recognizedAtom.Sequence, recognizedAtom.Sequence);
                                            scannedAtom = new ScannedAtom<Lexicon>(recognizedAtom.Code, line, column, recognizedAtom.Sequence);

                                            break;
                                        case Lexicon.IntegerNumericConstant:
                                        case Lexicon.FloatNumericConstant:
                                        case Lexicon.StringConstant:
                                        case Lexicon.CharConstant:
                                            if (!constants.TryGetValue(recognizedAtom.Sequence, out value))
                                                constants.Add(recognizedAtom.Sequence, recognizedAtom.Sequence);
                                            scannedAtom = new ScannedAtom<Lexicon>(recognizedAtom.Code, line, column, recognizedAtom.Sequence);

                                            break;
                                        default:
                                            scannedAtom = new ScannedAtom<Lexicon>(recognizedAtom.Code, line, column);
                                            break;
                                    }
                                    scannedAtoms.Add(scannedAtom);
                                }

                                wasPreviousDelimiter = recognizedAtom.IsDelimiter;
                                currentIndex += recognizedAtom.Sequence.Length;
                            }
                            else
                                throw new ArgumentException(string.Format("Unknown '{0}' at line: {1}, column: {2}", recognizedAtom.Sequence, line, column));
                        else
                            throw new ArgumentException(string.Format("Unknown symbol '{0}' at line: {1}, column: {2}", text[currentIndex], line, column));
                    }

                    return new ScanResult<Lexicon>(scannedAtoms, new ReadOnlyDictionary<string, string>(identifiers), new ReadOnlyDictionary<string, string>(constants));
                }
                else
                    throw new ArgumentNullException("language");
            else
                throw new ArgumentNullException("text");
        }

        private AtomRecognitionResult<Lexicon> _RecognizeAtom(string text, int currentIndex, bool canBeNonDelimiter)
        {
            return (from atomRecognizer in _atomRecognizers
                    let recognisitionResult = atomRecognizer.Recognize(text, currentIndex)
                    where recognisitionResult.Success
                    orderby recognisitionResult.Sequence.Length descending
                    select recognisitionResult).FirstOrDefault();
        }
    }
}
