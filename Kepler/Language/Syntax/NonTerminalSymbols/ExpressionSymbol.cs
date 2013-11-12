using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Extensions;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.TerminalSymbols;

namespace Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols
{
    public sealed class ExpressionSymbol
        : NonTerminalSymbol
    {
        private sealed class Node
        {
            public Node(ScannedAtom<Lexicon> atom, ILanguage<Lexicon> language)
            {
                if (atom != null)
                    if (language != null)
                    {
                        _atom = atom;
                        _language = language;
                        switch (atom.Code)
                        {
                            case Lexicon.Star:
                            case Lexicon.Percentage:
                            case Lexicon.Slash:
                            case Lexicon.Backslash:
                            case Lexicon.Plus:
                            case Lexicon.Minus:
                            case Lexicon.LessThan:
                            case Lexicon.LessThanOrEqualTo:
                            case Lexicon.Equal:
                            case Lexicon.GreaterThanOrEqualTo:
                            case Lexicon.GreaterThan:
                            case Lexicon.Negation:
                            case Lexicon.And:
                            case Lexicon.Or:
                                _isOperator = true;
                                break;
                            case Lexicon.OpeningRoundParenthesis:
                            case Lexicon.ClosingRoundParenthesis:
                                _isParenthesis = true;
                                break;
                            default:
                                throw new ArgumentException("Invalid atom: " + atom.Code);
                        }
                    }
                    else
                        throw new ArgumentNullException("language");
                else
                    throw new ArgumentNullException("atom");
            }

            public Node(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language)
            {
                if (atoms != null)
                    if (language != null)
                    {
                        _atoms = atoms;
                        _language = language;
                    }
                    else
                        throw new ArgumentNullException("language");
                else
                    throw new ArgumentNullException("atoms");
            }

            public Symbol ToSymbol()
            {
                if (IsOperator)
                    return new OperatorSymbol(_atom, _language);
                else
                    if (IsParenthesis)
                        if (_atom.Code == Lexicon.OpeningRoundParenthesis)
                            return new OpeningRoundParenthesisSymbol(_atom, _language);
                        else
                            return new ClosingRoundParenthesisSymbol(_atom, _language);
                    else
                        if (_atoms.Count == 1)
                        {
                            ConstantSymbol constantSymbol;

                            if (ConstantSymbol.TryCreate(_atoms[0], out constantSymbol))
                                return constantSymbol;
                            else
                                return new IdentifierSymbol(_atoms[0]);
                        }
                        else
                        {
                            FunctionCallSymbol functionCallSymbol;

                            if (FunctionCallSymbol.TryCreate(_atoms, _language, out functionCallSymbol))
                                return functionCallSymbol;
                            else
                                return new QualifiedIdentifierSymbol(_atoms, _language);
                        }
            }

            public ScannedAtom<Lexicon> Atom
            {
                get
                {
                    if (IsOperator || IsParenthesis)
                        return _atom;
                    else
                        throw new InvalidOperationException();
                }
            }

            public IReadOnlyList<ScannedAtom<Lexicon>> Atoms
            {
                get
                {
                    if (IsOperand)
                        return _atoms;
                    else
                        throw new InvalidOperationException();
                }
            }

            public int GetPriority()
            {
                if (IsOperator)
                    return _language.Operators.Values.First(op => op.Code == _atom.Code).Priority;
                else
                    throw new InvalidOperationException();
            }

            public bool IsOperator
            {
                get
                {
                    return _isOperator;
                }
            }

            public bool IsParenthesis
            {
                get
                {
                    return _isParenthesis;
                }
            }

            public bool IsOperand
            {
                get
                {
                    return (!_isOperator && !_isParenthesis);
                }
            }

            private readonly bool _isOperator = false;
            private readonly bool _isParenthesis = false;
            private readonly ScannedAtom<Lexicon> _atom;
            private readonly ILanguage<Lexicon> _language;
            private readonly IReadOnlyList<ScannedAtom<Lexicon>> _atoms;
        }

        static private IReadOnlyList<Symbol> _GetEvaluationOrderList(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language)
        {
            int previousCount;
            LinkedList<Node> nodes = _ToPostFixedForm(_GetNodes(atoms, language), language);

            do
            {
                LinkedListNode<Node> nodeToExpand = nodes.First;

                previousCount = nodes.Count;
                while (nodeToExpand != null)
                    if (nodeToExpand.Value.IsOperand)
                    {
                        foreach (Node spawnedNode in _ToPostFixedForm(_GetNodes(nodeToExpand.Value.Atoms, language), language))
                            nodes.AddBefore(nodeToExpand, spawnedNode);
                        if (nodeToExpand.Next != null)
                        {
                            nodeToExpand = nodeToExpand.Next;
                            nodes.Remove(nodeToExpand.Previous);
                        }
                        else
                        {
                            nodes.Remove(nodeToExpand);
                            nodeToExpand = null;
                        }
                    }
                    else
                        nodeToExpand = nodeToExpand.Next;
            } while (previousCount < nodes.Count);

            return new ReadOnlyCollection<Symbol>(nodes.Select(node => node.ToSymbol()).ToList());
        }

        static private LinkedList<Node> _ToPostFixedForm(LinkedList<Node> nodes, ILanguage<Lexicon> language)
        {
            LinkedList<Node> postfixedNodes = new LinkedList<Node>();
            IDictionary<Lexicon, int> prioritizedOperators = language.Operators.Values.ToDictionary(op => op.Code, op => op.Priority);
            Queue<Node> operands = new Queue<Node>();
            Stack<Node> operators = new Stack<Node>();

            foreach (Node node in nodes)
                if (node.IsOperand)
                    operands.Enqueue(node);
                else
                    if (operators.Count == 0
                        || prioritizedOperators[operators.Peek().Atom.Code] >= prioritizedOperators[node.Atom.Code])
                        operators.Push(node);
                    else
                    {
                        while (operands.Count > 0)
                            postfixedNodes.AddLast(operands.Dequeue());
                        while (operators.Count > 0)
                            postfixedNodes.AddLast(operators.Pop());
                        operators.Push(node);
                    }

            while (operands.Count > 0)
                postfixedNodes.AddLast(operands.Dequeue());
            while (operators.Count > 0)
                postfixedNodes.AddLast(operators.Pop());

            return postfixedNodes;
        }

        static private IReadOnlyList<ScannedAtom<Lexicon>> _Normalize(IReadOnlyList<ScannedAtom<Lexicon>> operandAtoms, ILanguage<Lexicon> language)
        {
            int currentIndex = 0;
            OpeningRoundParenthesisSymbol openingParenthesis;
            ClosingRoundParenthesisSymbol closingParenthesis;

            while (currentIndex < operandAtoms.Count
                   && OpeningRoundParenthesisSymbol.TryCreate(operandAtoms[currentIndex], language, out openingParenthesis))
                currentIndex++;

            int openParenthesisCount = currentIndex;

            currentIndex = operandAtoms.Count - 1;
            while (currentIndex >= 0
                   && ClosingRoundParenthesisSymbol.TryCreate(operandAtoms[currentIndex], language, out closingParenthesis))
                currentIndex--;

            int closedParenthesisCount = operandAtoms.Count - 1 - currentIndex;
            int extraParenthesisPairCount = Math.Min(openParenthesisCount, closedParenthesisCount);

            return operandAtoms.Sublist(extraParenthesisPairCount, operandAtoms.Count - extraParenthesisPairCount);
        }

        static private LinkedList<Node> _GetNodes(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language)
        {
            int operandStartIndex = 0, openParenthesis = 0;
            IReadOnlyList<ScannedAtom<Lexicon>> normalizedAtoms = _Normalize(atoms, language);
            LinkedList<Node> nodes = new LinkedList<Node>();

            for (int currentIndex = 0; currentIndex < normalizedAtoms.Count; currentIndex++)
                if (_IsOperator(normalizedAtoms[currentIndex].Code) && openParenthesis == 0)
                {
                    if (operandStartIndex < currentIndex)
                        nodes.AddLast(new Node(normalizedAtoms.Sublist(operandStartIndex, currentIndex - operandStartIndex), language));
                    nodes.AddLast(new Node(normalizedAtoms[currentIndex], language));
                    operandStartIndex = (currentIndex + 1);
                }
                else
                    if (normalizedAtoms[currentIndex].Code == Lexicon.OpeningRoundParenthesis)
                        openParenthesis++;
                    else
                        if (normalizedAtoms[currentIndex].Code == Lexicon.ClosingRoundParenthesis)
                        {
                            openParenthesis--;
                            if (openParenthesis < 0)
                                throw ExceptionFactory.CreateUnexpectedSymbol(")", normalizedAtoms[currentIndex].Line, normalizedAtoms[currentIndex].Column);
                        }
            if (operandStartIndex < normalizedAtoms.Count)
                nodes.AddLast(new Node(normalizedAtoms.Sublist(operandStartIndex, normalizedAtoms.Count - operandStartIndex), language));

            return nodes;
        }

        static private bool _IsOperator(Lexicon lexicon)
        {
            switch (lexicon)
            {
                case Lexicon.Star:
                case Lexicon.Percentage:
                case Lexicon.Slash:
                case Lexicon.Backslash:
                case Lexicon.Plus:
                case Lexicon.Minus:
                case Lexicon.LessThan:
                case Lexicon.LessThanOrEqualTo:
                case Lexicon.Equal:
                case Lexicon.GreaterThanOrEqualTo:
                case Lexicon.GreaterThan:
                case Lexicon.Negation:
                case Lexicon.And:
                case Lexicon.Or:
                    return true;
                default:
                    return false;
            }
        }

        public ExpressionSymbol(IReadOnlyList<ScannedAtom<Lexicon>> atoms, ILanguage<Lexicon> language, int startIndex = 0)
            : base(SymbolNodeType.Expression)
        {
            Exception exception = Validate(atoms, startIndex, 1);

            if (exception == null)
                if (language != null)
                {
                    _atoms = atoms;
                    _evaluationOrder = _GetEvaluationOrderList(atoms, language);
                }
                else
                    throw new ArgumentNullException("language");
            else
                throw exception;
        }

        public override IReadOnlyList<Symbol> Symbols
        {
            get
            {
                return _evaluationOrder;
            }
        }

        public IReadOnlyList<Symbol> EvaluationOrder
        {
            get
            {
                return _evaluationOrder;
            }
        }

        public override uint Line
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override uint Column
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private readonly IReadOnlyList<Symbol> _evaluationOrder;
        private readonly IReadOnlyList<ScannedAtom<Lexicon>> _atoms;
    }
}
