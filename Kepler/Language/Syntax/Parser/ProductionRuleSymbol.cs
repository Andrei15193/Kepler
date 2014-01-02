using System;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public sealed class ProductionRuleSymbol
	{
		public ProductionRuleSymbol(KeplerLanguage.ProductionRuleCode code, bool isSequence = false)
		{
			_isSequence = isSequence;
			_isTerminal = false;
			_terminalCode = 0;
			_nonTerminalCode = code;
		}
		public ProductionRuleSymbol(KeplerLanguage.AtomCode code, bool isSequence = false)
		{
			_isSequence = isSequence;
			_isTerminal = true;
			_terminalCode = code;
			_nonTerminalCode = 0;
		}

		public bool IsSequence
		{
			get
			{
				return _isSequence;
			}
		}
		public bool IsTerminal
		{
			get
			{
				return _isTerminal;
			}
		}
		public KeplerLanguage.AtomCode TerminalCode
		{
			get
			{
				if (!_isTerminal)
					throw new InvalidOperationException("The production rule symbol instance is not terminal!");
				return _terminalCode;
			}
		}
		public KeplerLanguage.ProductionRuleCode NonTerminalCode
		{
			get
			{
				if (_isTerminal)
					throw new InvalidOperationException("The production rule symbol instance is terminal!");
				return _nonTerminalCode;
			}
		}

		private readonly bool _isSequence;
		private readonly bool _isTerminal;
		private readonly KeplerLanguage.AtomCode _terminalCode;
		private readonly KeplerLanguage.ProductionRuleCode _nonTerminalCode;
	}
}