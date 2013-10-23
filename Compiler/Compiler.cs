namespace Andrei15193.Kepler.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public sealed class Compiler<TCode>
        where TCode : struct
    {
        public interface ILanguage
        {
            bool TryGetIdentifierCode(string text, out TCode code);

            bool TryGetConstantCode(string text, out TCode code);

            bool TryGetKeyWordCode(string text, out TCode code);

            bool TryGetIgnoreCode(string text, out TCode code);

            bool IsReservedWord(string text);

            bool CanIgnore(string text);

            bool CanIgnore(TCode code);

            IReadOnlyDictionary<string, TCode> Operators { get; }

            IReadOnlyDictionary<string, TCode> Separators { get; }

            IEnumerable<string> ReservedWords { get; }

            IEnumerable<Enclosure> Enclosures { get; }
        }

        public interface ILexicalAnalyzer
        {
            LexicalAnalysisResult Analyze(string text, Compiler<TCode>.ILanguage language);
        }

        public class Enclosure
        {
            public Enclosure(string openingSymbol, string closingSymbol)
            {
                if (openingSymbol != null)
                    if (closingSymbol != null)
                        if (openingSymbol.Length > 0)
                            if (closingSymbol.Length > 0)
                            {
                                _openingSymbol = openingSymbol;
                                _closingSymbol = closingSymbol;
                            }
                            else
                                throw new ArgumentException("The length of the closingSymbol cannot be zero.");
                        else
                            throw new ArgumentException("The length of the openingSymbol cannot be zero.");
                    else
                        throw new ArgumentNullException("closingSymbol");
                else
                    throw new ArgumentNullException("openingSymbol");
            }

            public virtual int IndexOfEnclosure(string text, out string enclosedText, int startIndex = 0)
            {
                if (text != null)
                {
                    enclosedText = null;
                    int indexOfOpeningSybol = text.IndexOf(_openingSymbol, startIndex);

                    if (indexOfOpeningSybol != -1)
                    {
                        int currentIndex = indexOfOpeningSybol + _openingSymbol.Length;

                        while (currentIndex < text.Length
                               && string.Compare(text, currentIndex, _closingSymbol, 0, _closingSymbol.Length) != 0)
                            if (text[currentIndex] == '\\')
                                if (string.Compare(text, currentIndex + 1, _closingSymbol, 0, _closingSymbol.Length) == 0)
                                    currentIndex += (1 + _closingSymbol.Length);
                                else
                                    currentIndex += 2;
                            else
                                currentIndex++;

                        if (currentIndex < text.Length)
                        {
                            enclosedText = text.Substring(indexOfOpeningSybol, currentIndex + _closingSymbol.Length - indexOfOpeningSybol);
                            return indexOfOpeningSybol;
                        }
                        else
                            return -1;
                    }
                    else
                        return -1;
                }
                else
                    throw new ArgumentNullException("text");
            }

            public int IndexOfEnclosure(string text, int startIndex = 0)
            {
                string enclosedText;

                return IndexOfEnclosure(text, out enclosedText, startIndex);
            }

            public string OpeningSymbol
            {
                get
                {
                    return _openingSymbol;
                }
            }

            public string ClosingSybol
            {
                get
                {
                    return _closingSymbol;
                }
            }

            private readonly string _openingSymbol;
            private readonly string _closingSymbol;
        }

        [Serializable]
        public sealed class LexicalAnalysisResult
        {
            public LexicalAnalysisResult(IReadOnlyList<Compiler<TCode>.ScannedAtom> scannedAtoms, IReadOnlyDictionary<string, string> identifiers, IReadOnlyDictionary<string, string> constants)
            {
                if (scannedAtoms != null)
                    if (identifiers != null)
                        if (constants != null)
                        {
                            _scannedAtoms = new ReadOnlyCollection<Compiler<TCode>.ScannedAtom>(scannedAtoms.ToList());
                            _identifiers = new ReadOnlyDictionary<string, string>(new SortedDictionary<string, string>(identifiers.ToDictionary(entry => entry.Key, entry => entry.Value)));
                            _constants = new ReadOnlyDictionary<string, string>(new SortedDictionary<string, string>(constants.ToDictionary(entry => entry.Key, entry => entry.Value)));
                        }
                        else
                            throw new ArgumentNullException("constants");
                    else
                        throw new ArgumentNullException("identifiers");
                else
                    throw new ArgumentNullException("scannedAtoms");
            }

            public IReadOnlyList<Compiler<TCode>.ScannedAtom> ScannedAtoms
            {
                get
                {
                    return _scannedAtoms;
                }
            }

            public IReadOnlyDictionary<string, string> Constants
            {
                get
                {
                    return _constants;
                }
            }

            public IReadOnlyDictionary<string, string> Identifiers
            {
                get
                {
                    return _identifiers;
                }
            }

            private readonly IReadOnlyList<Compiler<TCode>.ScannedAtom> _scannedAtoms;
            private readonly IReadOnlyDictionary<string, string> _identifiers;
            private readonly IReadOnlyDictionary<string, string> _constants;
        }

        [Serializable]
        public sealed class ScannedAtom
        {
            public ScannedAtom(TCode code, uint line, uint column, string value = null)
            {
                _code = code;
                _line = line;
                _column = column;
                _value = value;
            }

            public TCode Code
            {
                get
                {
                    return _code;
                }
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

            public string Value
            {
                get
                {
                    return _value;
                }
            }

            public bool HasValue
            {
                get
                {
                    return (_value != null);
                }
            }

            private readonly uint _line;
            private readonly uint _column;
            private readonly TCode _code;
            private readonly string _value;
        }

        private sealed class LexicalAnalyser : Compiler<TCode>.ILexicalAnalyzer
        {
            private class LexicalAnalysisResultBuilder
            {
                public LexicalAnalysisResultBuilder()
                {
                    _line = 0;
                    _column = 0;
                    _scannedAtoms = new List<Compiler<TCode>.ScannedAtom>();
                    _identifiers = new SortedDictionary<string, string>();
                    _constants = new SortedDictionary<string, string>();
                }

                public void AppendIdentifier(string identifier, TCode code)
                {
                    if (identifier != null)
                    {
                        if (!_identifiers.ContainsKey(identifier))
                            _identifiers.Add(identifier, identifier);
                        _scannedAtoms.Add(new Compiler<TCode>.ScannedAtom(code, _line, _column, identifier));
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
                        _scannedAtoms.Add(new Compiler<TCode>.ScannedAtom(code, _line, _column, constant));

                        int indexOfLastNewLine = 0, newLineCount = 0;

                        for (int currentIndex = constant.IndexOf(Environment.NewLine); currentIndex != -1; indexOfLastNewLine = currentIndex, currentIndex = constant.IndexOf(Environment.NewLine, currentIndex + Environment.NewLine.Length))
                            newLineCount++;
                        if (newLineCount > 0)
                        {
                            _line += (uint)newLineCount;
                            _column = (uint)(constant.Length - indexOfLastNewLine);
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
                        _scannedAtoms.Add(new Compiler<TCode>.ScannedAtom(code, _line, _column));
                        if (delimiter == Environment.NewLine)
                        {
                            _line++;
                            _column = 0;
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
                        _scannedAtoms.Add(new Compiler<TCode>.ScannedAtom(code, _line, _column));
                        _column += (uint)keyWord.Length;
                    }
                    else
                        throw new ArgumentNullException("keyWord");
                }

                public void AppendIgnoredSequence(string text, TCode code)
                {
                    if (text != null)
                    {
                        _scannedAtoms.Add(new Compiler<TCode>.ScannedAtom(code, _line, _column, text));

                        int indexOfLastNewLine = 0, newLineCount = 0;

                        for (int currentIndex = text.IndexOf(Environment.NewLine); currentIndex != -1; indexOfLastNewLine = currentIndex, currentIndex = text.IndexOf(Environment.NewLine, currentIndex + Environment.NewLine.Length))
                            newLineCount++;
                        if (newLineCount > 0)
                        {
                            _line += (uint)newLineCount;
                            _column = (uint)(text.Length - indexOfLastNewLine);
                        }
                        else
                            _column += (uint)text.Length;
                    }
                    else
                        throw new ArgumentNullException("text");
                }

                public Compiler<TCode>.LexicalAnalysisResult ToLexicalAnalysisResult(Func<ScannedAtom, bool> filter = null)
                {
                    if (filter == null)
                        return new Compiler<TCode>.LexicalAnalysisResult(_scannedAtoms,
                                                                         new ReadOnlyDictionary<string, string>(_identifiers),
                                                                         new ReadOnlyDictionary<string, string>(_constants));
                    else
                        return new Compiler<TCode>.LexicalAnalysisResult(_scannedAtoms.Where(filter)
                                                                                      .ToList(),
                                                                         new ReadOnlyDictionary<string, string>(_identifiers),
                                                                         new ReadOnlyDictionary<string, string>(_constants));
                }

                public void Clear()
                {
                    _line = 0;
                    _column = 0;
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
                private readonly List<Compiler<TCode>.ScannedAtom> _scannedAtoms;
                private readonly IDictionary<string, string> _identifiers;
                private readonly IDictionary<string, string> _constants;
            }

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

            static private int _IndexOfAny(string str, IEnumerable<Compiler<TCode>.Enclosure> enclosures, out string enclosedText, int startIndex = 0)
            {
                enclosedText = null;
                int foundIndex = -1;

                foreach (Compiler<TCode>.Enclosure enclosure in enclosures)
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

            public Compiler<TCode>.LexicalAnalysisResult Analyze(string text, Compiler<TCode>.ILanguage language)
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

            private void _AnalyzeEnclosures(string text, Compiler<TCode>.ILanguage language, LexicalAnalysisResultBuilder builder)
            {
                int previousIndex = 0;

                while (previousIndex < text.Length)
                {
                    string enclosedText;
                    int indexOfEnclosedText = _IndexOfAny(text, language.Enclosures, out enclosedText, previousIndex);

                    if (indexOfEnclosedText != -1)
                    {
                        if (previousIndex < indexOfEnclosedText)
                            _AnalyzeSeparators(text.Substring(previousIndex, indexOfEnclosedText - previousIndex), language, builder);
                        _AnalyzeTextLeaf(enclosedText, language, builder);
                        previousIndex = indexOfEnclosedText + enclosedText.Length;
                    }
                    else
                    {
                        _AnalyzeSeparators(text.Substring(previousIndex), language, builder);
                        previousIndex = text.Length;
                    }
                }
            }

            private void _AnalyzeSeparators(string text, Compiler<TCode>.ILanguage language, LexicalAnalysisResultBuilder builder)
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
                            _AnalyzeOperators(text.Substring(previousIndex, indexOfSeparator - previousIndex), language, builder);
                        builder.AppendDelimiter(separator, language.Separators[separator]);
                        previousIndex = indexOfSeparator + separator.Length;
                    }
                    else
                    {
                        _AnalyzeOperators(text.Substring(previousIndex), language, builder);
                        previousIndex = text.Length;
                    }
                }
            }

            private void _AnalyzeOperators(string text, Compiler<TCode>.ILanguage language, LexicalAnalysisResultBuilder builder)
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
                            _AnalyzeTextLeaf(text.Substring(previousIndex, indexOfOp - previousIndex), language, builder);
                        builder.AppendDelimiter(op, language.Operators[op]);
                        previousIndex = indexOfOp + op.Length;
                    }
                    else
                    {
                        _AnalyzeTextLeaf(text.Substring(previousIndex), language, builder);
                        previousIndex = text.Length;
                    }
                }
            }

            private void _AnalyzeTextLeaf(string text, ILanguage language, LexicalAnalysisResultBuilder builder)
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

        static public Compiler<TCode>.ILexicalAnalyzer DefaultLexicalAnalyzer
        {
            get
            {
                if (_defaultLexicalAnalyzer == null)
                    _defaultLexicalAnalyzer = new LexicalAnalyser();
                return _defaultLexicalAnalyzer;
            }
        }

        public Compiler(Compiler<TCode>.ILanguage language, Compiler<TCode>.ILexicalAnalyzer lexicalAnalyser = null)
        {
            if (language != null)
            {
                _language = language;
                _lexicalAnalyser = lexicalAnalyser ?? DefaultLexicalAnalyzer;
            }
            else
                throw new ArgumentNullException("language");
        }

        public LexicalAnalysisResult LexicallyAnalyze(string text)
        {
            if (text != null)
                return _lexicalAnalyser.Analyze(text, _language);
            else
                throw new ArgumentNullException("text");
        }

        public Compiler<TCode>.ILexicalAnalyzer LexicalAnalyzer
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

        public Compiler<TCode>.ILanguage Language
        {
            get
            {
                return _language;
            }
        }

        static private Compiler<TCode>.ILexicalAnalyzer _defaultLexicalAnalyzer = null;

        private Compiler<TCode>.ILexicalAnalyzer _lexicalAnalyser;
        private readonly Compiler<TCode>.ILanguage _language;
    }
}

