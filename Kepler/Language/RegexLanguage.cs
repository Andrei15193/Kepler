using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis.Attributes;

namespace Andrei15193.Kepler.Language
{
    internal class RegexLanguage<TCode>
        : ILanguage<TCode>
        where TCode : struct
    {
        static private bool _TryGetCode(string text, IReadOnlyDictionary<Regex, TCode> dictionary, out TCode code)
        {
            if (text != null)
                if (dictionary != null)
                {
                    bool hasMatches = false;

                    code = (from entry in dictionary
                            let result = new
                            {
                                Match = entry.Key.Match(text),
                                Code = entry.Value
                            }
                            where result.Match.Success
                            orderby result.Match.Value.Length descending
                            select result.Code).FirstOrDefault(result => (hasMatches = true));

                    return hasMatches;
                }
                else
                    throw new ArgumentNullException("dictionary");
            else
                throw new ArgumentNullException("text");
        }

        public RegexLanguage()
        {
            IDictionary<string, Operator<TCode>> operators = new SortedDictionary<string, Operator<TCode>>();
            IList<TCode> codesToIgnore = new List<TCode>();
            ISet<string> literalsToIgnore = new HashSet<string>();
            ISet<string> reservedWords = new HashSet<string>();
            IDictionary<TCode, string> literalSymbols = new Dictionary<TCode, string>();
            var literals = new Dictionary<AtomAttribute.LiteralType, IDictionary<string, TCode>>();
            var patterns = new Dictionary<AtomAttribute.PatternType, IDictionary<Regex, TCode>>();
            var enclosures = new Dictionary<AtomAttribute.EnclosureType, IDictionary<Enclosure, TCode>>();
            literals.Add(AtomAttribute.LiteralType.KeyWord, new SortedDictionary<string, TCode>());
            literals.Add(AtomAttribute.LiteralType.Separator, new SortedDictionary<string, TCode>());
            patterns.Add(AtomAttribute.PatternType.Constant, new Dictionary<Regex, TCode>());
            patterns.Add(AtomAttribute.PatternType.Identifier, new Dictionary<Regex, TCode>());
            patterns.Add(AtomAttribute.PatternType.Ignore, new Dictionary<Regex, TCode>());
            enclosures.Add(AtomAttribute.EnclosureType.Comment, new Dictionary<Enclosure, TCode>());
            enclosures.Add(AtomAttribute.EnclosureType.Constant, new Dictionary<Enclosure, TCode>());

            foreach (FieldInfo fieldInfo in typeof(TCode).GetFields(BindingFlags.Static | BindingFlags.Public))
                if (typeof(TCode).IsAssignableFrom(fieldInfo.FieldType))
                {
                    TCode fieldValue = (TCode)fieldInfo.GetValue(null);
                    AtomAttribute atomAttribute = fieldInfo.GetCustomAttribute<AtomAttribute>();

                    LiteralAtomAttribute literalAtomAttribute = atomAttribute as LiteralAtomAttribute;
                    if (literalAtomAttribute != null)
                    {
                        literalSymbols.Add(fieldValue, literalAtomAttribute.Literal);
                        if (literalAtomAttribute.IsReservedWord)
                            reservedWords.Add(literalAtomAttribute.Literal);
                        if (!literalAtomAttribute.ConsiderAtom)
                        {
                            literalsToIgnore.Add(literalAtomAttribute.Literal);
                            codesToIgnore.Add(fieldValue);
                        }
                        if (literalAtomAttribute.AtomType == AtomAttribute.LiteralType.Operator)
                            operators.Add(literalAtomAttribute.Literal, new Operator<TCode>(fieldValue, (fieldInfo.GetCustomAttribute<PriorityAttribute>() ?? PriorityAttribute.Default).Priority));
                        else
                            literals[literalAtomAttribute.AtomType].Add(literalAtomAttribute.Literal, fieldValue);
                    }
                    else
                    {
                        PatternAtomAttribute patternAtomAttribute = atomAttribute as PatternAtomAttribute;

                        if (patternAtomAttribute != null)
                        {
                            patterns[patternAtomAttribute.AtomType].Add(patternAtomAttribute.Regex, fieldValue);
                            if (patternAtomAttribute.AtomType == AtomAttribute.PatternType.Ignore)
                                codesToIgnore.Add(fieldValue);
                        }
                        else
                        {
                            EnclosedAtomAttribute enclosedAtomAttribute = atomAttribute as EnclosedAtomAttribute;

                            if (enclosedAtomAttribute != null)
                            {
                                enclosures[enclosedAtomAttribute.AtomType].Add(new Enclosure(enclosedAtomAttribute.OpeningSymbol, enclosedAtomAttribute.ClosingSymbol, enclosedAtomAttribute.InnerSequenceLength), fieldValue);
                                if (enclosedAtomAttribute.AtomType == AtomAttribute.EnclosureType.Comment)
                                    codesToIgnore.Add(fieldValue);
                            }
                        }
                    }
                }

            _keyWords = new ReadOnlyDictionary<string, TCode>(literals[AtomAttribute.LiteralType.KeyWord]);
            _operators = new ReadOnlyDictionary<string, Operator<TCode>>(operators);
            _separators = new ReadOnlyDictionary<string, TCode>(literals[AtomAttribute.LiteralType.Separator]);
            _constants = new ReadOnlyDictionary<Regex, TCode>(patterns[AtomAttribute.PatternType.Constant]);
            _identifiers = new ReadOnlyDictionary<Regex, TCode>(patterns[AtomAttribute.PatternType.Identifier]);
            _ignorePatterns = new ReadOnlyDictionary<Regex, TCode>(patterns[AtomAttribute.PatternType.Ignore]);
            _reservedWords = new ReadOnlyCollection<string>(reservedWords.ToList());
            _patterns = new ReadOnlyCollection<Regex>(_constants.Keys
                                                                .Concat(_identifiers.Keys)
                                                                .Concat(_ignorePatterns.Keys)
                                                                .ToList());
            _ignoreLiterals = new ReadOnlyCollection<string>(literalsToIgnore.ToList());
            _ignoreCodes = new ReadOnlyCollection<TCode>(codesToIgnore);
            _enclosedConstants = new ReadOnlyDictionary<Enclosure, TCode>(enclosures[AtomAttribute.EnclosureType.Constant]);
            _enclosedComments = new ReadOnlyDictionary<Enclosure, TCode>(enclosures[AtomAttribute.EnclosureType.Comment]);
            _literalSymbols = new ReadOnlyDictionary<TCode, string>(literalSymbols);
            _literalCodes = new ReadOnlyDictionary<string, TCode>(new SortedDictionary<string, TCode>(literalSymbols.ToDictionary(pair => pair.Value, pair => pair.Key)));
        }

        public bool TryGetIdentifierCode(string text, out TCode code)
        {
            return _TryGetCode(text, _identifiers, out code);
        }

        public bool TryGetConstantCode(string text, out TCode code)
        {
            if (_TryGetCode(text, _constants, out code))
                return true;
            else
            {
                string enclosedText;
                var firstEnclosure = _enclosedConstants.Keys
                                                       .FirstOrDefault(enclosure => (enclosure.IndexOfEnclosure(text, out enclosedText) != -1
                                                                                     && enclosedText.Length == text.Length));

                return (firstEnclosure != null
                        && _enclosedConstants.TryGetValue(firstEnclosure, out code));
            }
        }

        public bool TryGetKeyWordCode(string text, out TCode code)
        {
            return _keyWords.TryGetValue(text, out code);
        }

        public bool TryGetIgnoreCode(string text, out TCode code)
        {
            Operator<TCode> op;
            code = default(TCode);

            if (text != null)
            {
                bool isLiteralIgnored = _ignoreLiterals.Contains(text);

                if (isLiteralIgnored
                    && (_keyWords.TryGetValue(text, out code)
                        || _separators.TryGetValue(text, out code)))
                    return true;
                else
                    if (isLiteralIgnored
                        && _operators.TryGetValue(text, out op))
                    {
                        code = op.Code;
                        return true;
                    }
                    else
                    {
                        string enclosedText;
                        var firstEnclosure = _enclosedComments.Keys
                                                              .FirstOrDefault(enclosure => (enclosure.IndexOfEnclosure(text, out enclosedText) != -1
                                                                                            && enclosedText.Length == text.Length));

                        if (firstEnclosure != null
                            && _enclosedComments.TryGetValue(firstEnclosure, out code))
                            return true;
                        else
                            using (IEnumerator<KeyValuePair<Regex, TCode>> ignorePattern = _ignorePatterns.GetEnumerator())
                            {
                                bool hasCurrent = ignorePattern.MoveNext();

                                while (hasCurrent && !ignorePattern.Current.Key.IsMatch(text))
                                    hasCurrent = ignorePattern.MoveNext();

                                if (hasCurrent)
                                {
                                    code = ignorePattern.Current.Value;
                                    return true;
                                }
                                else
                                    return false;
                            }
                    }
            }
            else
                throw new ArgumentNullException("text");
        }

        public bool TryGetSymbol(TCode code, out string symbol)
        {
            return _literalSymbols.TryGetValue(code, out symbol);
        }

        public bool CanIgnore(string text)
        {
            TCode code;

            return TryGetIgnoreCode(text, out code);
        }

        public bool CanIgnore(TCode code)
        {
            return _ignoreCodes.Contains(code);
        }

        public string GetSymbol(TCode code)
        {
            string symbol;

            if (_literalSymbols.TryGetValue(code, out symbol))
                return symbol;
            else
                throw new ArgumentException(code.ToString() + " does not represent a code for a literal symbol!");
        }

        public TCode GetCode(string symbol)
        {
            return _literalSymbols.FirstOrDefault(literalSymbol => literalSymbol.Value == symbol).Key;
        }

        public bool IsReservedWord(string text)
        {
            return _reservedWords.Contains(text);
        }

        public IReadOnlyDictionary<string, Operator<TCode>> Operators
        {
            get
            {
                return _operators;
            }
        }

        public IReadOnlyDictionary<string, TCode> Separators
        {
            get
            {
                return _separators;
            }
        }

        public IEnumerable<Enclosure> Enclosures
        {
            get
            {
                return _enclosedComments.Keys
                                        .Concat(_enclosedConstants.Keys)
                                        .ToList();
            }
        }

        public IEnumerable<string> ReservedWords
        {
            get
            {
                return _reservedWords;
            }
        }

        private readonly IReadOnlyCollection<string> _reservedWords;
        private readonly IReadOnlyCollection<TCode> _ignoreCodes;
        private readonly IReadOnlyCollection<string> _ignoreLiterals;
        private readonly IReadOnlyCollection<Regex> _patterns;
        private readonly IReadOnlyDictionary<string, TCode> _keyWords;
        private readonly IReadOnlyDictionary<string, Operator<TCode>> _operators;
        private readonly IReadOnlyDictionary<string, TCode> _separators;
        private readonly IReadOnlyDictionary<Regex, TCode> _constants;
        private readonly IReadOnlyDictionary<Regex, TCode> _identifiers;
        private readonly IReadOnlyDictionary<Regex, TCode> _ignorePatterns;
        private readonly IReadOnlyDictionary<Enclosure, TCode> _enclosedConstants;
        private readonly IReadOnlyDictionary<Enclosure, TCode> _enclosedComments;
        private readonly IReadOnlyDictionary<TCode, string> _literalSymbols;
        private readonly IReadOnlyDictionary<string, TCode> _literalCodes;
    }
}
