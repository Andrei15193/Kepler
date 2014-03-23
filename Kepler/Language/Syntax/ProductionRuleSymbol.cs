using System;
namespace Andrei15193.Kepler.Language.Syntax
{
	public struct ProductionRuleSymbol<TAtomCode, TProductionRuleCode>
		: IEquatable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>>
		where TAtomCode : struct
		where TProductionRuleCode : struct
	{
		public ProductionRuleSymbol(TProductionRuleCode nonTerminal, bool skipRootParsedNode = false)
		{
			_skipRootParsedNode = skipRootParsedNode;
			_isTerminal = false;
			_terminalCode = default(TAtomCode);
			_nonTerminalCode = nonTerminal;
		}
		public ProductionRuleSymbol(TAtomCode terminal, bool skipRootParsedNode = false)
		{
			_skipRootParsedNode = skipRootParsedNode;
			_isTerminal = true;
			_terminalCode = terminal;
			_nonTerminalCode = default(TProductionRuleCode);
		}

		public static bool operator ==(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, object right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, ValueType right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, IEquatable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(object left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator ==(ValueType left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator ==(IEquatable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator !=(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, object right)
		{
			return !(left == right);
		}
		public static bool operator !=(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, ValueType right)
		{
			return !(left == right);
		}
		public static bool operator !=(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> left, IEquatable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> right)
		{
			return !(left == right);
		}
		public static bool operator !=(object left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(ValueType left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(IEquatable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> left, ProductionRuleSymbol<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}

		#region IEquatable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> Members
		public bool Equals(ProductionRuleSymbol<TAtomCode, TProductionRuleCode> other)
		{
			if (_isTerminal && other._isTerminal)
				return (_terminalCode.Equals(other._terminalCode));
			else
				if (!_isTerminal && !other._isTerminal)
					return (_nonTerminalCode.Equals(other._nonTerminalCode));
				else
					return false;
		}
		#endregion
		public override string ToString()
		{
			if (_isTerminal)
				return string.Join(_terminalCode.ToString(), "\"", "\"");
			else
				return _nonTerminalCode.ToString();
		}
		public override bool Equals(object obj)
		{
			return (obj is ProductionRuleSymbol<TAtomCode, TProductionRuleCode> && Equals((ProductionRuleSymbol<TAtomCode, TProductionRuleCode>)obj));
		}
		public override int GetHashCode()
		{
			if (_isTerminal)
				return _terminalCode.GetHashCode();
			else
				return _nonTerminalCode.GetHashCode();
		}
		public bool SkipRootParsedNode
		{
			get
			{
				return _skipRootParsedNode;
			}
		}
		public bool IsTerminal
		{
			get
			{
				return _isTerminal;
			}
		}
		public TAtomCode TerminalCode
		{
			get
			{
				if (!_isTerminal)
					throw new InvalidOperationException("The production rule symbol instance is not terminal!");
				return _terminalCode;
			}
		}
		public TProductionRuleCode NonTerminalCode
		{
			get
			{
				if (_isTerminal)
					throw new InvalidOperationException("The production rule symbol instance is terminal!");
				return _nonTerminalCode;
			}
		}

		private readonly bool _isTerminal;
		private readonly bool _skipRootParsedNode;
		private readonly TAtomCode _terminalCode;
		private readonly TProductionRuleCode _nonTerminalCode;
	}
}