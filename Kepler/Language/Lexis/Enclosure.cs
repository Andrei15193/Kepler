using System;

namespace Andrei15193.Kepler.Language.Lexis
{
    public class Enclosure
    {
        static private int _SkipEscapedChars(string text, int startIndex, int sequenceLength)
        {
            if (0 <= startIndex && startIndex < text.Length)
                if (sequenceLength >= 0)
                {
                    bool isBackslashEscaped = false;
                    int currentIndex = startIndex;

                    for (int characterCount = 0; currentIndex < text.Length && characterCount < sequenceLength; currentIndex++)
                        if (text[currentIndex] != '\\' || isBackslashEscaped)
                        {
                            characterCount++;
                            isBackslashEscaped = false;
                        }
                        else
                            isBackslashEscaped = true;

                    return currentIndex;
                }
                else
                    throw new ArgumentException("The length cannot be negative!");
            else
                throw new ArgumentOutOfRangeException("startIndex");
        }

        public Enclosure(string openingSymbol, string closingSymbol, uint? innerSequenceLength = null)
        {
            if (openingSymbol != null)
                if (closingSymbol != null)
                    if (openingSymbol.Length > 0)
                        if (closingSymbol.Length > 0)
                        {
                            _openingSymbol = openingSymbol;
                            _closingSymbol = closingSymbol;
                            _innerSequenceLength = innerSequenceLength;
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

                if (indexOfOpeningSybol != -1 && indexOfOpeningSybol + _openingSymbol.Length + _closingSymbol.Length < text.Length)
                    if (_innerSequenceLength.HasValue)
                    {
                        int innerSequenceEndIndex = _SkipEscapedChars(text, indexOfOpeningSybol + _openingSymbol.Length, (int)_innerSequenceLength.Value);

                        while (indexOfOpeningSybol != -1
                               && innerSequenceEndIndex + _closingSymbol.Length <= text.Length
                               && string.Compare(text, innerSequenceEndIndex, _closingSymbol, 0, _closingSymbol.Length) != 0)
                        {
                            indexOfOpeningSybol = text.IndexOf(_openingSymbol, indexOfOpeningSybol + _openingSymbol.Length);
                            if (indexOfOpeningSybol != -1)
                                innerSequenceEndIndex = _SkipEscapedChars(text, indexOfOpeningSybol, (int)_innerSequenceLength.Value);
                        }
                        if (indexOfOpeningSybol != -1 && innerSequenceEndIndex + _closingSymbol.Length <= text.Length)
                            enclosedText = text.Substring(indexOfOpeningSybol, innerSequenceEndIndex + _closingSymbol.Length - indexOfOpeningSybol);
                    }
                    else
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
                            enclosedText = text.Substring(indexOfOpeningSybol, currentIndex + _closingSymbol.Length - indexOfOpeningSybol);
                    }

                return (enclosedText == null ? -1 : indexOfOpeningSybol);
            }
            else
                throw new ArgumentNullException("text");
        }

        public int IndexOfEnclosure(string text, int startIndex = 0)
        {
            string enclosedText;

            return IndexOfEnclosure(text, out enclosedText, startIndex);
        }

        public uint GetInnerSequenceLength()
        {
            if (_innerSequenceLength.HasValue)
                return _innerSequenceLength.Value;
            else
                throw new InvalidOperationException("There is no inner sequence length!");
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

        public bool HasInnerSequenceLength
        {
            get
            {
                return _innerSequenceLength.HasValue;
            }
        }

        private readonly string _openingSymbol;
        private readonly string _closingSymbol;
        private readonly uint? _innerSequenceLength;
    }
}
