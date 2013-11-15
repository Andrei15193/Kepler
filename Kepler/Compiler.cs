using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Language;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax;

namespace Andrei15193.Kepler
{
    public sealed class Compiler<TCode>
        where TCode : struct
    {
        static private readonly ILexicalAnalyser<TCode> _defaultLexicalAnalyser = new SplitBasedLexicalAnalyser();

        static public ILexicalAnalyser<TCode> DefaultLexicalAnalyser
        {
            get
            {
                return _defaultLexicalAnalyser;
            }
        }

        private sealed class SplitBasedLexicalAnalyser
            : ILexicalAnalyser<TCode>
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
                    _scannedAtoms = new List<ScannedAtom<TCode>>();
                    _identifiers = new SortedDictionary<string, string>();
                    _constants = new SortedDictionary<string, string>();
                }

                public void AppendIdentifier(string identifier, TCode code)
                {
                    if (identifier != null)
                    {
                        if (!_identifiers.ContainsKey(identifier))
                            _identifiers.Add(identifier, identifier);
                        _scannedAtoms.Add(new ScannedAtom<TCode>(code, _line, _column, identifier));
                        _column += (uint)identifier.Length;
                    }
                    else
                        throw new ArgumentNullException("identifier");
                }

                public void AppendConstant(string constant, TCode code)
                {
                    if (constant != null)
                    {
                        if (!_constants.ContainsKey(constant))
                            _constants.Add(constant, constant);
                        _scannedAtoms.Add(new ScannedAtom<TCode>(code, _line, _column, constant));

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

                public void AppendDelimiter(string delimiter, TCode code)
                {
                    if (delimiter != null)
                    {
                        _scannedAtoms.Add(new ScannedAtom<TCode>(code, _line, _column));
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

                public void AppendKeyWord(string keyWord, TCode code)
                {
                    if (keyWord != null)
                    {
                        _scannedAtoms.Add(new ScannedAtom<TCode>(code, _line, _column));
                        _column += (uint)keyWord.Length;
                    }
                    else
                        throw new ArgumentNullException("keyWord");
                }

                public void AppendIgnoredSequence(string text, TCode code)
                {
                    if (text != null)
                    {
                        _scannedAtoms.Add(new ScannedAtom<TCode>(code, _line, _column, text));

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

                public ScanResult<TCode> ToLexicalAnalysisResult(Func<ScannedAtom<TCode>, bool> filter = null)
                {
                    if (filter == null)
                        return new ScanResult<TCode>(_scannedAtoms,
                                                                new ReadOnlyDictionary<string, string>(_identifiers),
                                                                new ReadOnlyDictionary<string, string>(_constants));
                    else
                        return new ScanResult<TCode>(_scannedAtoms.Where(filter)
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
                private readonly List<ScannedAtom<TCode>> _scannedAtoms;
                private readonly IDictionary<string, string> _identifiers;
                private readonly IDictionary<string, string> _constants;
            }

            public ScanResult<TCode> Scan(string text, ILanguage<TCode> language)
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

            private void _AnalyzeEnclosures(string text, ILanguage<TCode> language, LexicalAnalysisResultBuilder builder)
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

            private void _AnalyzeOperators(string text, ILanguage<TCode> language, LexicalAnalysisResultBuilder builder)
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

            private void _AnalyzeSeparators(string text, ILanguage<TCode> language, LexicalAnalysisResultBuilder builder)
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

            private void _AnalyzeTextLeaf(string text, ILanguage<TCode> language, LexicalAnalysisResultBuilder builder)
            {
                TCode code;

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

        public Compiler(RuleSet<TCode> ruleSet, ILexicalAnalyser<TCode> lexicalAnalyser = null, ILanguage<TCode> language = null)
        {
            if (ruleSet != null)
            {
                _ruleSet = ruleSet;
                _lexicalAnalyser = (lexicalAnalyser ?? DefaultLexicalAnalyser);
                _language = (language ?? Language<TCode>.Default);
            }
            else
                throw new ArgumentNullException("ruleSet");
        }

        public ScanResult<TCode> Scan(string text)
        {
            if (text != null)
                return _lexicalAnalyser.Scan(text, _language);
            else
                throw new ArgumentNullException("text");
        }

        public ParsedNode<TCode> Parse(string text)
        {
            return _ruleSet.Parse(Scan(text).ScannedAtoms);
        }

        public ParsedNode<TCode> Parse(ScanResult<TCode> scanResult)
        {
            if (scanResult != null)
                return _ruleSet.Parse(scanResult.ScannedAtoms);
            else
                throw new ArgumentNullException("scanResult");
        }

        public ILexicalAnalyser<TCode> LexicalAnalyser
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

        public ILanguage<TCode> Language
        {
            get
            {
                return _language;
            }
        }

        private ILexicalAnalyser<TCode> _lexicalAnalyser;
        private readonly ILanguage<TCode> _language;
        private readonly RuleSet<TCode> _ruleSet;
    }
}
