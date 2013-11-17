using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler
{
    public sealed class Compiler
    {
        static private readonly ILexicalAnalyser<Lexicon> _defaultLexicalAnalyser = new StateMachineLexicalAnalyser();

        static public ILexicalAnalyser<Lexicon> DefaultLexicalAnalyser
        {
            get
            {
                return _defaultLexicalAnalyser;
            }
        }

        private sealed class SplitBasedLexicalAnalyser
            : ILexicalAnalyser<Lexicon>
        {
            static private int _IndexOfAny(string str, IEnumerable<string> values, out string foundValue, int startIndex = 0)
            {
                foundValue = null;
                int foundIndex = -1;

                foreach (string value in values)
                {
                    int indexOfValue = str.IndexOf(value, startIndex);

                    if (indexOfValue != -1
                        && (foundIndex == -1
                            || indexOfValue < foundIndex))
                    {
                        foundIndex = indexOfValue;
                        foundValue = value;
                    }
                }

                return foundIndex;
            }

            static private int _IndexOfAny(string str, IEnumerable<Enclosure> enclosures, out string enclosedText, int startIndex = 0)
            {
                enclosedText = null;
                int foundIndex = -1;

                foreach (Enclosure enclosure in enclosures)
                {
                    string enclosed;
                    int indexOfEnclosed = enclosure.IndexOfEnclosure(str, out enclosed, startIndex);

                    if (indexOfEnclosed != -1
                        && (foundIndex == -1
                            || indexOfEnclosed < foundIndex))
                    {
                        foundIndex = indexOfEnclosed;
                        enclosedText = enclosed;
                    }
                }

                return foundIndex;
            }

            private class LexicalAnalysisResultBuilder
            {
                public LexicalAnalysisResultBuilder()
                {
                    _line = 1;
                    _column = 1;
                    _scannedAtoms = new List<ScannedAtom<Lexicon>>();
                    _identifiers = new SortedDictionary<string, string>();
                    _constants = new SortedDictionary<string, string>();
                }

                public void AppendIdentifier(string identifier, Lexicon code)
                {
                    if (identifier != null)
                    {
                        if (!_identifiers.ContainsKey(identifier))
                            _identifiers.Add(identifier, identifier);
                        _scannedAtoms.Add(new ScannedAtom<Lexicon>(code, _line, _column, identifier));
                        _column += (uint)identifier.Length;
                    }
                    else
                        throw new ArgumentNullException("identifier");
                }

                public void AppendConstant(string constant, Lexicon code)
                {
                    if (constant != null)
                    {
                        if (!_constants.ContainsKey(constant))
                            _constants.Add(constant, constant);
                        _scannedAtoms.Add(new ScannedAtom<Lexicon>(code, _line, _column, constant));

                        int indexOfLastNewLine = 0, newLineCount = 0;

                        for (int currentIndex = constant.IndexOf(Environment.NewLine); currentIndex != -1; indexOfLastNewLine = currentIndex, currentIndex = constant.IndexOf(Environment.NewLine, currentIndex + Environment.NewLine.Length))
                            newLineCount++;
                        if (newLineCount > 0)
                        {
                            _line += (uint)newLineCount;
                            _column = (uint)(constant.Length - indexOfLastNewLine) + 1;
                        }
                        else
                            _column += (uint)constant.Length;
                    }
                    else
                        throw new ArgumentNullException("constant");
                }

                public void AppendDelimiter(string delimiter, Lexicon code)
                {
                    if (delimiter != null)
                    {
                        _scannedAtoms.Add(new ScannedAtom<Lexicon>(code, _line, _column));
                        if (delimiter == Environment.NewLine)
                        {
                            _line++;
                            _column = 1;
                        }
                        else
                            _column += (uint)delimiter.Length;
                    }
                    else
                        throw new ArgumentNullException("delimiter");
                }

                public void AppendKeyWord(string keyWord, Lexicon code)
                {
                    if (keyWord != null)
                    {
                        _scannedAtoms.Add(new ScannedAtom<Lexicon>(code, _line, _column));
                        _column += (uint)keyWord.Length;
                    }
                    else
                        throw new ArgumentNullException("keyWord");
                }

                public void AppendIgnoredSequence(string text, Lexicon code)
                {
                    if (text != null)
                    {
                        _scannedAtoms.Add(new ScannedAtom<Lexicon>(code, _line, _column, text));

                        int indexOfLastNewLine = 0, newLineCount = 0;

                        for (int currentIndex = text.IndexOf(Environment.NewLine); currentIndex != -1; indexOfLastNewLine = currentIndex, currentIndex = text.IndexOf(Environment.NewLine, currentIndex + Environment.NewLine.Length))
                            newLineCount++;
                        if (newLineCount > 0)
                        {
                            _line += (uint)newLineCount;
                            _column = (uint)(text.Length - indexOfLastNewLine) + 1;
                        }
                        else
                            _column += (uint)text.Length;
                    }
                    else
                        throw new ArgumentNullException("text");
                }

                public ScanResult<Lexicon> ToLexicalAnalysisResult(Func<ScannedAtom<Lexicon>, bool> filter = null)
                {
                    if (filter == null)
                        return new ScanResult<Lexicon>(_scannedAtoms,
                                                                new ReadOnlyDictionary<string, string>(_identifiers),
                                                                new ReadOnlyDictionary<string, string>(_constants));
                    else
                        return new ScanResult<Lexicon>(_scannedAtoms.Where(filter)
                                                                             .ToList(),
                                                                new ReadOnlyDictionary<string, string>(_identifiers),
                                                                new ReadOnlyDictionary<string, string>(_constants));
                }

                public void Clear()
                {
                    _line = 1;
                    _column = 1;
                    _scannedAtoms.Clear();
                    _identifiers.Clear();
                    _constants.Clear();
                }

                public uint Line
                {
                    get
                    {
                        return _line;
                    }
                }

                public uint Column
                {
                    get
                    {
                        return _column;
                    }
                }

                private uint _line;
                private uint _column;
                private readonly List<ScannedAtom<Lexicon>> _scannedAtoms;
                private readonly IDictionary<string, string> _identifiers;
                private readonly IDictionary<string, string> _constants;
            }

            public ScanResult<Lexicon> Scan(string text, ILanguage<Lexicon> language)
            {
                if (text != null)
                    if (language != null)
                    {
                        LexicalAnalysisResultBuilder builder = new LexicalAnalysisResultBuilder();
                        _AnalyzeEnclosures(text, language, builder);
                        return builder.ToLexicalAnalysisResult(scannedAtom => ((scannedAtom.HasValue && !language.CanIgnore(scannedAtom.Value))
                                                                               || !language.CanIgnore(scannedAtom.Code)));
                    }
                    else
                        throw new ArgumentNullException("language");
                else
                    throw new ArgumentNullException("text");
            }

            private void _AnalyzeEnclosures(string text, ILanguage<Lexicon> language, LexicalAnalysisResultBuilder builder)
            {
                int previousIndex = 0;

                while (previousIndex < text.Length)
                {
                    string enclosedText;
                    int indexOfEnclosedText = _IndexOfAny(text, language.Enclosures, out enclosedText, previousIndex);

                    if (indexOfEnclosedText != -1)
                    {
                        if (previousIndex < indexOfEnclosedText)
                            _AnalyzeOperators(text.Substring(previousIndex, indexOfEnclosedText - previousIndex), language, builder);
                        _AnalyzeTextLeaf(enclosedText, language, builder);
                        previousIndex = indexOfEnclosedText + enclosedText.Length;
                    }
                    else
                    {
                        _AnalyzeOperators(text.Substring(previousIndex), language, builder);
                        previousIndex = text.Length;
                    }
                }
            }

            private void _AnalyzeOperators(string text, ILanguage<Lexicon> language, LexicalAnalysisResultBuilder builder)
            {
                int previousIndex = 0;
                IEnumerable<string> operators = language.Operators
                                                        .Keys
                                                        .OrderByDescending(operatorSymbol => operatorSymbol.Length)
                                                        .ToList();

                while (previousIndex < text.Length)
                {
                    string op;
                    int indexOfOp = _IndexOfAny(text, operators, out op, previousIndex);

                    if (indexOfOp != -1)
                    {
                        if (previousIndex < indexOfOp)
                            _AnalyzeSeparators(text.Substring(previousIndex, indexOfOp - previousIndex), language, builder);
                        builder.AppendDelimiter(op, language.Operators[op].Code);
                        previousIndex = indexOfOp + op.Length;
                    }
                    else
                    {
                        _AnalyzeSeparators(text.Substring(previousIndex), language, builder);
                        previousIndex = text.Length;
                    }
                }
            }

            private void _AnalyzeSeparators(string text, ILanguage<Lexicon> language, LexicalAnalysisResultBuilder builder)
            {
                int previousIndex = 0;
                IEnumerable<string> separators = language.Separators
                                                         .Keys
                                                         .OrderByDescending(separatorSymbol => separatorSymbol.Length)
                                                         .ToList();

                while (previousIndex < text.Length)
                {
                    string separator;
                    int indexOfSeparator = _IndexOfAny(text, separators, out separator, previousIndex);

                    if (indexOfSeparator != -1)
                    {
                        if (previousIndex < indexOfSeparator)
                            _AnalyzeTextLeaf(text.Substring(previousIndex, indexOfSeparator - previousIndex), language, builder);
                        builder.AppendDelimiter(separator, language.Separators[separator]);
                        previousIndex = indexOfSeparator + separator.Length;
                    }
                    else
                    {
                        _AnalyzeTextLeaf(text.Substring(previousIndex), language, builder);
                        previousIndex = text.Length;
                    }
                }
            }

            private void _AnalyzeTextLeaf(string text, ILanguage<Lexicon> language, LexicalAnalysisResultBuilder builder)
            {
                Lexicon code;

                if (language.TryGetIgnoreCode(text, out code))
                    builder.AppendIgnoredSequence(text, code);
                else
                    if (language.TryGetKeyWordCode(text, out code))
                        builder.AppendKeyWord(text, code);
                    else
                        if (language.TryGetConstantCode(text, out code))
                            builder.AppendConstant(text, code);
                        else
                            if (language.TryGetIdentifierCode(text, out code))
                                builder.AppendIdentifier(text, code);
                            else
                                throw new ArgumentException(string.Format("Unknown symbol: {0}{1}At line: {2}, column: {3}", text, Environment.NewLine, builder.Line, builder.Column));
            }
        }

        public Compiler(RuleSet<Lexicon> ruleSet, ILexicalAnalyser<Lexicon> lexicalAnalyser = null, ILanguage<Lexicon> language = null)
        {
            if (ruleSet != null)
            {
                _ruleSet = ruleSet;
                _lexicalAnalyser = (lexicalAnalyser ?? DefaultLexicalAnalyser);
                _language = (language ?? Language<Lexicon>.Default);
            }
            else
                throw new ArgumentNullException("ruleSet");
        }

        public ScanResult<Lexicon> Scan(string text)
        {
            if (text != null)
                return _lexicalAnalyser.Scan(text, _language);
            else
                throw new ArgumentNullException("text");
        }

        public ParsedNode<Lexicon> Parse(string text)
        {
            return _ruleSet.Parse(Scan(text).ScannedAtoms);
        }

        public ParsedNode<Lexicon> Parse(ScanResult<Lexicon> scanResult)
        {
            if (scanResult != null)
                return _ruleSet.Parse(scanResult.ScannedAtoms);
            else
                throw new ArgumentNullException("scanResult");
        }

        public ILexicalAnalyser<Lexicon> LexicalAnalyser
        {
            get
            {
                return _lexicalAnalyser;
            }
            set
            {
                if (value != null)
                    _lexicalAnalyser = value;
                else
                    throw new ArgumentNullException("LexicalAnalyzer");
            }
        }

        public ILanguage<Lexicon> Language
        {
            get
            {
                return _language;
            }
        }

        private ILexicalAnalyser<Lexicon> _lexicalAnalyser;
        private readonly ILanguage<Lexicon> _language;
        private readonly RuleSet<Lexicon> _ruleSet;
    }
}
