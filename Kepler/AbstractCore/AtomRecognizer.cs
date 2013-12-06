using System;

namespace Andrei15193.Kepler.AbstractCore
{
    public class AtomRecognizer<TCode>
        where TCode : struct
    {
        public AtomRecognizer(TCode code, State<char> initialState, bool isDelimiter)
        {
            if (initialState != null)
            {
                _initialState = initialState;
                _isDelimiter = isDelimiter;
                _code = code;
            }
            else
                throw new ArgumentNullException("initialState");
        }

        public virtual bool IsValid(string text, int startIndex = 0)
        {
            if (text != null)
            {
                int currentIndex = 0;
                State<char> currentState = _initialState;

                if (text.Length > 0)
                {
                    do
                    {
                        currentState = currentState.Transit(text[currentIndex]);
                        currentIndex++;
                    } while (currentIndex < text.Length && currentState != null);

                    return (currentState != null && currentState.IsFinalState && currentIndex == text.Length);
                }
                else
                    return _initialState.IsFinalState;
            }
            else
                throw new ArgumentNullException("sequence");
        }

        public virtual AtomRecognitionResult<TCode> Recognize(string text, int startIndex = 0)
        {
            if (text != null)
            {
                int currentIndex = startIndex, sequenceEndIndex = currentIndex;
                State<char> currentState, nextState = _initialState;

                if (text.Length > 0)
                {
                    while (currentIndex < text.Length && nextState != null)
                    {
                        currentState = nextState;
                        nextState = currentState.Transit(text[currentIndex]);
                        currentIndex++;
                        if (nextState != null && nextState.IsFinalState)
                            sequenceEndIndex = currentIndex;
                    }

                    if (sequenceEndIndex != startIndex)
                        return new AtomRecognitionResult<TCode>(text.Substring(startIndex, sequenceEndIndex - startIndex), _code, IsDelimiter);
                    else
                        return new AtomRecognitionResult<TCode>();
                }
                else
                    return new AtomRecognitionResult<TCode>();
            }
            else
                throw new ArgumentNullException("text");
        }

        public State<char> InitialState
        {
            get
            {
                return _initialState;
            }
        }

        public TCode Code
        {
            get
            {
                return _code;
            }
        }

        public bool IsDelimiter
        {
            get
            {
                return _isDelimiter;
            }
        }

        private readonly bool _isDelimiter;
        private readonly TCode _code;
        private readonly State<char> _initialState;
    }
}
