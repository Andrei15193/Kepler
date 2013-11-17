using System;
using System.Collections.Generic;

namespace Andrei15193.Kepler.AbstractCore
{
    public class AtomRecognizer<TCode>
        where TCode : struct
    {
        public AtomRecognizer(TCode code, State<char> initialState)
        {
            if (initialState != null)
            {
                _initialState = initialState;
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
                        return new AtomRecognitionResult<TCode>(text.Substring(startIndex, sequenceEndIndex - startIndex), _code);
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

        private readonly TCode _code;
        private readonly State<char> _initialState;
    }

    //public sealed class ConstantStateMachine
    //    : StateMachine<char>
    //{
    //    public ConstantStateMachine(State<char> initialState, IEnumerable<State<char>> states, IEnumerable<char> alphabet)
    //        : base(initialState)
    //    {
    //        if (states != null)
    //            if (alphabet != null)
    //            {
    //                List<State<char>> allStates = new List<State<char>>();

    //                if (!states.Contains(initialState))
    //                    allStates.Add(initialState);
    //                allStates.AddRange(states);
    //                _states = new ReadOnlyCollection<State<char>>(allStates.ToList());
    //                _alphabet = new ReadOnlyCollection<char>(alphabet.ToList());
    //            }
    //            else
    //                throw new ArgumentNullException("alphabet");
    //        else
    //            throw new ArgumentNullException("states");
    //    }

    //    public override bool IsValid(char character)
    //    {
    //        State<char> currentState = InitialState.Transit(character);

    //        return (currentState != null && currentState.IsFinalState);
    //    }

    //    public bool IsValid(string sequence)
    //    {
    //        if (sequence != null)
    //        {
    //            int currentIndex = 0;
    //            State<char> currentState = InitialState;

    //            if (sequence.Length > 0)
    //            {
    //                do
    //                {
    //                    currentState = currentState.Transit(sequence[currentIndex]);
    //                    currentIndex++;
    //                } while (currentIndex < sequence.Length && currentState != null);

    //                return (currentState != null && currentState.IsFinalState && currentIndex == sequence.Length);
    //            }
    //            else
    //                return InitialState.IsFinalState;
    //        }
    //        else
    //            throw new ArgumentNullException("sequence");
    //    }

    //    public string GetLongestPrefix(string sequence)
    //    {
    //        if (sequence != null)
    //        {
    //            int currentIndex = 0, sequenceLength = 0;
    //            State<char> currentState, nextState = InitialState;

    //            if (sequence.Length > 0)
    //            {
    //                while (currentIndex < sequence.Length && nextState != null)
    //                {
    //                    currentState = nextState;
    //                    nextState = currentState.Transit(sequence[currentIndex]);
    //                    currentIndex++;
    //                    if (nextState != null && nextState.IsFinalState)
    //                        sequenceLength = currentIndex;
    //                }
    //                return sequence.Substring(0, sequenceLength);
    //            }
    //            else
    //                return string.Empty;
    //        }
    //        else
    //            throw new ArgumentNullException("sequence");
    //    }

    //    public override IReadOnlyList<State<char>> States
    //    {
    //        get
    //        {
    //            return _states;
    //        }
    //    }

    //    public override IReadOnlyList<Transition<char>> Transitions
    //    {
    //        get
    //        {
    //            IDictionary<string, State<char>> visitedStates = new SortedDictionary<string, State<char>>();
    //            HashSet<State<char>> statesToVisit = new HashSet<State<char>>() { InitialState };
    //            IList<Transition<char>> transitions = new List<Transition<char>>();

    //            while (statesToVisit.Count > 0)
    //            {
    //                State<char> currentState = statesToVisit.First();

    //                statesToVisit.Remove(currentState);
    //                visitedStates.Add(currentState.Name, currentState);
    //                foreach (Transition<char> transtion in currentState.Transitions)
    //                {
    //                    transitions.Add(transtion);
    //                    if (!visitedStates.ContainsKey(transtion.Destination.Name))
    //                        statesToVisit.Add(transtion.Destination);
    //                }
    //            }

    //            return new ReadOnlyCollection<Transition<char>>(transitions);
    //        }
    //    }

    //    public override IReadOnlyList<char> Alphabet
    //    {
    //        get
    //        {
    //            return _alphabet;
    //        }
    //    }

    //    private readonly IReadOnlyList<State<char>> _states;
    //    private readonly IReadOnlyList<char> _alphabet;
    //}
}
