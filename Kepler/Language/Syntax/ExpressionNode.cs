using System;
using System.Collections.Generic;
using System.Linq;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
	public sealed class ExpressionNode
	{
		static private ExpressionElement _CreateUnaryPrefixedOperatorElement(Lexicon code)
		{
			switch (code)
			{
				case Lexicon.Plus:
					return new OperatorElement(OperationName.IntegerPromotion);
				case Lexicon.Minus:
					return new OperatorElement(OperationName.AdditiveInverse);
				case Lexicon.Negation:
					return new OperatorElement(OperationName.Negation);
				default:
					throw new ArgumentException("Expected unary prefixed operator code!");
			}
		}

		static private ExpressionElement _CreateBinaryOperatorElement(Lexicon code)
		{
			switch (code)
			{
				case Lexicon.Star:
					return new OperatorElement(OperationName.Multiplication);
				case Lexicon.Percentage:
					return new OperatorElement(OperationName.Modulo);
				case Lexicon.Slash:
					return new OperatorElement(OperationName.Division);
				case Lexicon.Backslash:
					return new OperatorElement(OperationName.IntegerDivision);
				case Lexicon.Plus:
					return new OperatorElement(OperationName.Addition);
				case Lexicon.Minus:
					return new OperatorElement(OperationName.Subtraction);
				case Lexicon.LessThan:
					return new OperatorElement(OperationName.LessThanComparison);
				case Lexicon.LessThanOrEqualTo:
					return new OperatorElement(OperationName.LessThanOrEqualToComparison);
				case Lexicon.Equal:
					return new OperatorElement(OperationName.EqualComparison);
				case Lexicon.GreaterThanOrEqualTo:
					return new OperatorElement(OperationName.GreaterThanOrEqualToComparison);
				case Lexicon.GreaterThan:
					return new OperatorElement(OperationName.GreaterThanComparison);
				case Lexicon.And:
					return new OperatorElement(OperationName.Conjuction);
				case Lexicon.Or:
					return new OperatorElement(OperationName.Disjunction);
				default:
					throw new ArgumentException("Expected operator code!");
			}
		}

		static private ExpressionElement _CreateOperandElement(ParsedNode<Lexicon> operandParsedNode)
		{
			switch (operandParsedNode.ChildNodeGroups[0])
			{
				case KeplerRuleSet.FunctionCall:
					return new OperandElement.FunctionCall(new FunctionCallStatementNode(operandParsedNode[KeplerRuleSet.FunctionCall, 0]));
				case KeplerRuleSet.QualifiedIdentifier:
					return new OperandElement.Accessor(new QualifiedIdentifierNode(operandParsedNode[KeplerRuleSet.QualifiedIdentifier, 0]));
				case KeplerRuleSet.ArithmeticConstant:
				case KeplerRuleSet.BooleanConstant:
					return new OperandElement.Constant(operandParsedNode[operandParsedNode.ChildNodeGroups[0], 0]);
				default:
					throw new ArgumentException("Expeted function call, qualified identifier or cosntant!");
			}
		}

		private sealed class ElementToExpand
		  : ExpressionElement
		{
			public ElementToExpand(ParsedNode<Lexicon> node)
			{
				if (node != null)
					_node = node;
				else
					throw new ArgumentNullException("node");
			}

			/// <summary>
			/// This operation is not supported!
			/// </summary>
			/// <exception cref="System.NotImplementedException">Always thrown.</exception>
			public override ExpressionElementType ElementType
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public ParsedNode<Lexicon> ParsedNode
			{
				get
				{
					return _node;
				}
			}

			private readonly ParsedNode<Lexicon> _node;
		}

		internal ExpressionNode(ParsedNode<Lexicon> expressionParsedNode)
		{
			if (expressionParsedNode != null)
				if (expressionParsedNode.Name == KeplerRuleSet.Expression)
				{
					IReadOnlyList<ParsedNode<Lexicon>> arithmeticExpression;

					if (expressionParsedNode.TryGetChildNodeGroup(KeplerRuleSet.ArithmeticExpression, out arithmeticExpression))
						_elements = _GetFromArithmeticExpression(arithmeticExpression[0]);
					else
						_elements = _GetFromBooleanExpression(expressionParsedNode[KeplerRuleSet.BooleanExpression, 0]);
				}
				else
					if (expressionParsedNode.Name == KeplerRuleSet.ArithmeticExpression)
						_elements = _GetFromArithmeticExpression(expressionParsedNode);
					else
						if (expressionParsedNode.Name == KeplerRuleSet.BooleanExpression)
							_elements = _GetFromBooleanExpression(expressionParsedNode);
						else
							throw new ArgumentException("Must be expression node!", "expressionParsedNode");
			else
				throw new ArgumentNullException("expressionParsedNode");
		}

		internal ExpressionNode(IEnumerable<ExpressionElement> expressionElements)
		{
			if (expressionElements != null)
				_elements = expressionElements.ToList();
			else
				throw new ArgumentNullException("expressionElements");
		}

		internal ExpressionNode(params ExpressionElement[] expressionElements)
			: this((IEnumerable<ExpressionElement>)expressionElements)
		{
		}

		public override string ToString()
		{
			return string.Join(" ", _elements.Select(element => "{" + element.ToString() + "}"));
		}

		private IReadOnlyList<ExpressionElement> _GetFromArithmeticExpression(ParsedNode<Lexicon> parsedNode)
		{
			Queue<LinkedListNode<ExpressionElement>> elementsToExpand = new Queue<LinkedListNode<ExpressionElement>>();
			LinkedList<ExpressionElement> elements = new LinkedList<ExpressionElement>();
			elements.AddFirst(new ElementToExpand(parsedNode));
			elementsToExpand.Enqueue(elements.First);

			while (elementsToExpand.Count > 0)
			{
				IReadOnlyList<ParsedNode<Lexicon>> childNodeGroup;
				LinkedListNode<ExpressionElement> elementToExpand = elementsToExpand.Dequeue();
				ParsedNode<Lexicon> parsedNodeToExpand = ((ElementToExpand)elementToExpand.Value).ParsedNode;

				switch (parsedNodeToExpand.Name)
				{
					case KeplerRuleSet.ArithmeticOperand:
						elements.AddBefore(elementToExpand, _CreateOperandElement(parsedNodeToExpand));
						break;
					default:
						if (parsedNodeToExpand.TryGetChildNodeGroup(KeplerRuleSet.UnaryPrefixedArithmeticOperator, out childNodeGroup))
						{
							elements.AddBefore(elementToExpand, _CreateUnaryPrefixedOperatorElement(childNodeGroup[0].Atoms[0].Code));
							elements.AddBefore(elementToExpand, new ElementToExpand(parsedNodeToExpand[KeplerRuleSet.ArithmeticExpression, 0]));
							elementsToExpand.Enqueue(elementToExpand.Previous);
						}
						else
							if (parsedNodeToExpand.TryGetChildNodeGroup(KeplerRuleSet.BinaryArithmeticOperator, out childNodeGroup))
							{
								IReadOnlyList<ParsedNode<Lexicon>> expressionChildNodes = parsedNodeToExpand[KeplerRuleSet.ArithmeticExpression];

								elements.AddBefore(elementToExpand, new ElementToExpand(expressionChildNodes[0]));
								elementsToExpand.Enqueue(elementToExpand.Previous);
								elements.AddBefore(elementToExpand, _CreateBinaryOperatorElement(childNodeGroup[0].Atoms[0].Code));
								elements.AddBefore(elementToExpand, new ElementToExpand(expressionChildNodes[1]));
								elementsToExpand.Enqueue(elementToExpand.Previous);
							}
							else
								if (parsedNodeToExpand.TryGetChildNodeGroup(KeplerRuleSet.ArithmeticExpression, out childNodeGroup))
								{
									elements.AddBefore(elementToExpand, new ParenthesisElement(isOpenning: true));
									elements.AddBefore(elementToExpand, new ElementToExpand(childNodeGroup[0]));
									elementsToExpand.Enqueue(elementToExpand.Previous);
									elements.AddBefore(elementToExpand, new ParenthesisElement(isOpenning: false));
								}
								else
								{
									elements.AddBefore(elementToExpand, new ElementToExpand(parsedNodeToExpand[KeplerRuleSet.ArithmeticOperand, 0]));
									elementsToExpand.Enqueue(elementToExpand.Previous);
								}
						break;
				}

				elements.Remove(elementToExpand);
			}

			return elements.ToList();
		}

		private IReadOnlyList<ExpressionElement> _GetFromBooleanExpression(ParsedNode<Lexicon> parsedNode)
		{
			Queue<LinkedListNode<ExpressionElement>> elementsToExpand = new Queue<LinkedListNode<ExpressionElement>>();
			LinkedList<ExpressionElement> elements = new LinkedList<ExpressionElement>();
			elements.AddFirst(new ElementToExpand(parsedNode));
			elementsToExpand.Enqueue(elements.First);

			while (elementsToExpand.Count > 0)
			{
				IReadOnlyList<ParsedNode<Lexicon>> childNodeGroup;
				LinkedListNode<ExpressionElement> elementToExpand = elementsToExpand.Dequeue();
				ParsedNode<Lexicon> parsedNodeToExpand = ((ElementToExpand)elementToExpand.Value).ParsedNode;

				switch (parsedNodeToExpand.Name)
				{
					case KeplerRuleSet.BooleanOperand:
						elements.AddBefore(elementToExpand, _CreateOperandElement(parsedNodeToExpand));
						break;
					default:
						if (parsedNodeToExpand.TryGetChildNodeGroup(KeplerRuleSet.UnaryPrefixedBooleanOperator, out childNodeGroup))
						{
							elements.AddBefore(elementToExpand, _CreateUnaryPrefixedOperatorElement(childNodeGroup[0].Atoms[0].Code));
							elements.AddBefore(elementToExpand, new ElementToExpand(parsedNodeToExpand[KeplerRuleSet.BooleanExpression, 0]));
							elementsToExpand.Enqueue(elementToExpand.Previous);
						}
						else
							if (parsedNodeToExpand.TryGetChildNodeGroup(KeplerRuleSet.BinaryBooleanOperator, out childNodeGroup))
							{
								IReadOnlyList<ParsedNode<Lexicon>> expressionChildNodes = parsedNodeToExpand[KeplerRuleSet.BooleanExpression];

								elements.AddBefore(elementToExpand, new ElementToExpand(expressionChildNodes[0]));
								elementsToExpand.Enqueue(elementToExpand.Previous);
								elements.AddBefore(elementToExpand, _CreateBinaryOperatorElement(childNodeGroup[0].Atoms[0].Code));
								elements.AddBefore(elementToExpand, new ElementToExpand(expressionChildNodes[1]));
								elementsToExpand.Enqueue(elementToExpand.Previous);
							}
							else
								if (parsedNodeToExpand.TryGetChildNodeGroup(KeplerRuleSet.ArithmeticRelation, out childNodeGroup))
								{
									IReadOnlyList<ParsedNode<Lexicon>> expressionChildNodes = parsedNodeToExpand[KeplerRuleSet.ArithmeticExpression];

									foreach (ExpressionElement expressionElement in _GetFromArithmeticExpression(expressionChildNodes[0]))
										elements.AddBefore(elementToExpand, expressionElement);
									elements.AddBefore(elementToExpand, _CreateBinaryOperatorElement(childNodeGroup[0].Atoms[0].Code));
									foreach (ExpressionElement expressionElement in _GetFromArithmeticExpression(expressionChildNodes[1]))
										elements.AddBefore(elementToExpand, expressionElement);
								}
								else
									if (parsedNodeToExpand.TryGetChildNodeGroup(KeplerRuleSet.BooleanExpression, out childNodeGroup))
									{
										elements.AddBefore(elementToExpand, new ParenthesisElement(isOpenning: true));
										elements.AddBefore(elementToExpand, new ElementToExpand(childNodeGroup[0]));
										elementsToExpand.Enqueue(elementToExpand.Previous);
										elements.AddBefore(elementToExpand, new ParenthesisElement(isOpenning: false));
									}
									else
									{
										elements.AddBefore(elementToExpand, new ElementToExpand(parsedNodeToExpand[KeplerRuleSet.BooleanOperand, 0]));
										elementsToExpand.Enqueue(elementToExpand.Previous);
									}
						break;
				}

				elements.Remove(elementToExpand);
			}

			return elements.ToList();
		}

		public IReadOnlyList<ExpressionElement> Elements
		{
			get
			{
				return _elements;
			}
		}

		private readonly IReadOnlyList<ExpressionElement> _elements;
	}
}
