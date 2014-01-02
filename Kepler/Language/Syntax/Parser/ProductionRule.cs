using System;
using System.Collections.Generic;
using System.Linq;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public sealed class ProductionRule
	{
		public ProductionRule(KeplerLanguage.ProductionRuleCode code, IEnumerable<ProductionRuleSymbol> symbols)
		{
			_code = code;
			if (symbols == null)
				throw new ArgumentNullException("symbols");
			_symbols = symbols.Where(symbol => symbol != null).ToList();
		}
		public ProductionRule(KeplerLanguage.ProductionRuleCode code, params ProductionRuleSymbol[] symbols)
			: this(code, (IEnumerable<ProductionRuleSymbol>)symbols)
		{
		}

		public KeplerLanguage.ProductionRuleCode Code
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

		private KeplerLanguage.ProductionRuleCode _code;
		private readonly IReadOnlyList<ProductionRuleSymbol> _symbols;
	}
}