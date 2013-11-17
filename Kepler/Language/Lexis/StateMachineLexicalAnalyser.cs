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
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("assert", Lexicon.Assert));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("begin", Lexicon.Begin));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("do", Lexicon.Do));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("else", Lexicon.Else));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("end", Lexicon.End));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("fact", Lexicon.Fact));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("false", Lexicon.False));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("new", Lexicon.New));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("predicate", Lexicon.Predicate));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("skip", Lexicon.Skip));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("stop", Lexicon.Stop));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("then", Lexicon.Then));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("true", Lexicon.True));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("when", Lexicon.When));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("while", Lexicon.While));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("throw", Lexicon.Throw));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("try", Lexicon.Try));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("catch", Lexicon.Catch));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("finally", Lexicon.Finally));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("::", Lexicon.Scope));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("*", Lexicon.Star));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("%", Lexicon.Percentage));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("/", Lexicon.Slash));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\\", Lexicon.Backslash));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("+", Lexicon.Plus));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("-", Lexicon.Minus));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("<", Lexicon.LessThan));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("<=", Lexicon.LessThanOrEqualTo));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("=", Lexicon.Equal));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(">=", Lexicon.GreaterThanOrEqualTo));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(">", Lexicon.GreaterThan));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("negation", Lexicon.Negation));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("and", Lexicon.And));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("or", Lexicon.Or));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("(", Lexicon.OpeningRoundParenthesis));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(")", Lexicon.ClosingRoundParenthesis));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("[", Lexicon.OpeningSquareParenthesis));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("]", Lexicon.ClosingSquareParenthesis));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(":", Lexicon.Colon));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(",", Lexicon.Comma));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(".", Lexicon.Dot));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer(" ", Lexicon.WhiteSpace));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\t", Lexicon.Tabulator));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\n", Lexicon.LineFeed));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\r", Lexicon.CarriageReturn));
            _atomRecognizers.Add(_CreateLiteralAtomRecognizer("\r\n", Lexicon.NewLine));
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

            return new AtomRecognizer<Lexicon>(Lexicon.Comment, startState);
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

            return new AtomRecognizer<Lexicon>(Lexicon.MultilineComment, startState);
        }

        private static AtomRecognizer<Lexicon> _CreateLiteralAtomRecognizer(string literal, Lexicon code)
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

                    return new AtomRecognizer<Lexicon>(code, startState);
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

            return new AtomRecognizer<Lexicon>(Lexicon.Identifier, startState);
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

            return new AtomRecognizer<Lexicon>(Lexicon.IntegerNumericConstant, startState);
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

            return new AtomRecognizer<Lexicon>(Lexicon.FloatNumericConstant, startState);
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

            return new AtomRecognizer<Lexicon>(Lexicon.CharConstant, startState);
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

            return new AtomRecognizer<Lexicon>(Lexicon.StringConstant, startState);
        }

        static private char[] _escapedCharacters = new[] { 'r', 'n', 't' };
        static private IList<AtomRecognizer<Lexicon>> _atomRecognizers = new List<AtomRecognizer<Lexicon>>();

        public ScanResult<Lexicon> Scan(string text, ILanguage<Lexicon> language)
        {
            if (text != null)
                if (language != null)
                {
                    uint line = 1, column = 1;
                    int currentIndex = 0;
                    List<ScannedAtom<Lexicon>> scannedAtoms = new List<ScannedAtom<Lexicon>>();
                    IDictionary<string, string> identifiers = new SortedDictionary<string, string>();
                    IDictionary<string, string> constants = new SortedDictionary<string, string>();

                    while (currentIndex < text.Length)
                    {
                        AtomRecognitionResult<Lexicon> recognizedAtom = _RecognizeAtom(text, currentIndex);

                        if (recognizedAtom.Success)
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
                            currentIndex += recognizedAtom.Sequence.Length;
                        }
                        else
                            throw new ArgumentException(string.Format("Unknown '{0}' symbol at line: {1}, clumn: {2}", text[currentIndex], line, column));
                    }

                    return new ScanResult<Lexicon>(scannedAtoms, new ReadOnlyDictionary<string, string>(identifiers), new ReadOnlyDictionary<string, string>(constants));
                }
                else
                    throw new ArgumentNullException("language");
            else
                throw new ArgumentNullException("text");
        }

        private AtomRecognitionResult<Lexicon> _RecognizeAtom(string text, int currentIndex)
        {
            return (from atomRecognizer in _atomRecognizers
                    let recognisitionResult = atomRecognizer.Recognize(text, currentIndex)
                    where recognisitionResult.Success
                    orderby recognisitionResult.Sequence.Length descending
                    select recognisitionResult).FirstOrDefault();
        }
    }
}
