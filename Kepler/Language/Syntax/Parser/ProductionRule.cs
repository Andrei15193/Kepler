using System;
using System.Collections.Generic;
using System.Linq;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public struct ProductionRule
		: IEquatable<ProductionRule>
	{
		public ProductionRule(ProductionRuleCode code, IEnumerable<ProductionRuleSymbol> symbols)
		{
			if (symbols == null)
				throw new ArgumentNullException("symbols");
			_code = code;
			_symbols = symbols.ToList();
		}
		public ProductionRule(ProductionRuleCode code, params ProductionRuleSymbol[] symbols)
			: this(code, (IEnumerable<ProductionRuleSymbol>)symbols)
		{
		}
		
		#region IEquatable<ProductionRule> Members
		public bool Equals(ProductionRule other)
		{
			return (_code == other._code
					&& _symbols.SequenceEqual(other._symbols));
		}
		#endregion
		public ProductionRuleCode Code
		{
			get
			{
				return _code;
			}
		}
		public IReadOnlyList<ProductionRuleSymbol> Symbols
		{
			get
			{
				return _symbols;
			}
		}
		public override bool Equals(object obj)
		{
			return (obj is ProductionRule && Equals((ProductionRule)obj));
		}
		public override int GetHashCode()
		{
			return _symbols.Aggregate(_code.GetHashCode(), (hashCode, symbol) => (hashCode ^ symbol.GetHashCode()));
		}
		public override string ToString()
		{
			return string.Format("{0} -> {1}", _code.ToString(), string.Join(" ", _symbols.Select(symbol => symbol.ToString())));
		}

		private ProductionRuleCode _code;
		private readonly IReadOnlyList<ProductionRuleSymbol> _symbols;
	}
}