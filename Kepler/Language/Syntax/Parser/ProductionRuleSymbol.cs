using System;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public struct ProductionRuleSymbol
		: IEquatable<ProductionRuleSymbol>
	{
		public ProductionRuleSymbol(ProductionRuleCode nonTerminal, bool skipRootParsedNode = false)
		{
			_skipRootParsedNode = skipRootParsedNode;
			_isTerminal = false;
			_terminalCode = 0;
			_nonTerminalCode = nonTerminal;
		}
		public ProductionRuleSymbol(AtomCode terminal, bool skipRootParsedNode = false)
		{
			_skipRootParsedNode = skipRootParsedNode;
			_isTerminal = true;
			_terminalCode = terminal;
			_nonTerminalCode = 0;
		}

		#region IEquatable<ProductionRuleSymbol> Members
		public bool Equals(ProductionRuleSymbol other)
		{
			if (_isTerminal && other._isTerminal)
				return (_terminalCode == other._terminalCode);
			else
				if (!_isTerminal && !other._isTerminal)
					return (_nonTerminalCode == other._nonTerminalCode);
				else
					return false;
		}
		#endregion
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
		public AtomCode TerminalCode
		{
			get
			{
				if (!_isTerminal)
					throw new InvalidOperationException("The production rule symbol instance is not terminal!");
				return _terminalCode;
			}
		}
		public ProductionRuleCode NonTerminalCode
		{
			get
			{
				if (_isTerminal)
					throw new InvalidOperationException("The production rule symbol instance is terminal!");
				return _nonTerminalCode;
			}
		}
		public override string ToString()
		{
			if (_isTerminal)
				return string.Join(_terminalCode.ToString(), "\"", "\"");
			else
				return _nonTerminalCode.ToString();
		}
		public override bool Equals(object obj)
		{
			return (obj is ProductionRuleSymbol && Equals((ProductionRuleSymbol)obj));
		}
		public override int GetHashCode()
		{
			if (_isTerminal)
				return _terminalCode.GetHashCode();
			else
				return _nonTerminalCode.GetHashCode();
		}

		private readonly bool _isTerminal;
		private readonly bool _skipRootParsedNode;
		private readonly AtomCode _terminalCode;
		private readonly ProductionRuleCode _nonTerminalCode;

	}
}