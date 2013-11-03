using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Lexis.Attributes;

namespace Andrei15193.Kepler.Language
{
    public class RegexLanguage<TCode>
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
            IList<TCode> codesToIgnore = new List<TCode>();
            ISet<string> literalsToIgnore = new HashSet<string>();
            ISet<string> reservedWords = new HashSet<string>();
            var literals = new Dictionary<AtomAttribute.LiteralType, IDictionary<string, TCode>>();
            var patterns = new Dictionary<AtomAttribute.PatternType, IDictionary<Regex, TCode>>();
            var enclosures = new Dictionary<AtomAttribute.EnclosureType, IDictionary<Enclosure, TCode>>();
            literals.Add(AtomAttribute.LiteralType.KeyWord, new SortedDictionary<string, TCode>());
            literals.Add(AtomAttribute.LiteralType.Operator, new SortedDictionary<string, TCode>());
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
                        if (literalAtomAttribute.IsReservedWord)
                            reservedWords.Add(literalAtomAttribute.Literal);
                        if (!literalAtomAttribute.ConsiderAtom)
                        {
                            literalsToIgnore.Add(literalAtomAttribute.Literal);
                            codesToIgnore.Add(fieldValue);
                        }
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
            _operators = new ReadOnlyDictionary<string, TCode>(literals[AtomAttribute.LiteralType.Operator]);
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
            code = default(TCode);

            if (text != null)
                if (_ignoreLiterals.Contains(text)
                    && (_keyWords.TryGetValue(text, out code)
                        || _operators.TryGetValue(text, out code)
                        || _separators.TryGetValue(text, out code)))
                    return true;
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
            else
                throw new ArgumentNullException("text");
        }

        public bool IsReservedWord(string text)
        {
            return _reservedWords.Contains(text);
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

        public IReadOnlyDictionary<string, TCode> Operators
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
        private readonly IReadOnlyDictionary<string, TCode> _operators;
        private readonly IReadOnlyDictionary<string, TCode> _separators;
        private readonly IReadOnlyDictionary<Regex, TCode> _constants;
        private readonly IReadOnlyDictionary<Regex, TCode> _identifiers;
        private readonly IReadOnlyDictionary<Regex, TCode> _ignorePatterns;
        private readonly IReadOnlyDictionary<Enclosure, TCode> _enclosedConstants;
        private readonly IReadOnlyDictionary<Enclosure, TCode> _enclosedComments;
    }
}
