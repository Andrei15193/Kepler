using System;
using System.Collections.Generic;
using System.Linq;
namespace Andrei15193.Kepler.Language.Syntax
{
	public struct ProductionRule<TAtomCode, TProductionRuleCode>
		: IEquatable<ProductionRule<TAtomCode, TProductionRuleCode>>
		where TAtomCode : struct
		where TProductionRuleCode : struct
	{
		public ProductionRule(TProductionRuleCode code, IEnumerable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> symbols)
		{
			if (symbols == null)
				throw new ArgumentNullException("symbols");
			_code = code;
			_symbols = symbols.ToList();
		}
		public ProductionRule(TProductionRuleCode code, params ProductionRuleSymbol<TAtomCode, TProductionRuleCode>[] symbols)
			: this(code, (IEnumerable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>>)symbols)
		{
		}

		public static bool operator ==(ProductionRule<TAtomCode, TProductionRuleCode> left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ProductionRule<TAtomCode, TProductionRuleCode> left, object right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ProductionRule<TAtomCode, TProductionRuleCode> left, ValueType right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(ProductionRule<TAtomCode, TProductionRuleCode> left, IEquatable<ProductionRule<TAtomCode, TProductionRuleCode>> right)
		{
			return left.Equals(right);
		}
		public static bool operator ==(object left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator ==(ValueType left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator ==(IEquatable<ProductionRule<TAtomCode, TProductionRuleCode>> left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return right.Equals(left);
		}
		public static bool operator !=(ProductionRule<TAtomCode, TProductionRuleCode> left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(ProductionRule<TAtomCode, TProductionRuleCode> left, object right)
		{
			return !(left == right);
		}
		public static bool operator !=(ProductionRule<TAtomCode, TProductionRuleCode> left, ValueType right)
		{
			return !(left == right);
		}
		public static bool operator !=(ProductionRule<TAtomCode, TProductionRuleCode> left, IEquatable<ProductionRule<TAtomCode, TProductionRuleCode>> right)
		{
			return !(left == right);
		}
		public static bool operator !=(object left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(ValueType left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}
		public static bool operator !=(IEquatable<ProductionRule<TAtomCode, TProductionRuleCode>> left, ProductionRule<TAtomCode, TProductionRuleCode> right)
		{
			return !(left == right);
		}

		#region IEquatable<ProductionRule<TAtomCode, TProductionRuleCode>> Members
		public bool Equals(ProductionRule<TAtomCode, TProductionRuleCode> other)
		{
			return (_code.Equals(other._code)
					&& _symbols.SequenceEqual(other._symbols));
		}
		#endregion
		public override bool Equals(object obj)
		{
			return (obj is ProductionRule<TAtomCode, TProductionRuleCode> && Equals((ProductionRule<TAtomCode, TProductionRuleCode>)obj));
		}
		public override int GetHashCode()
		{
			return _symbols.Aggregate(_code.GetHashCode(), (hashCode, symbol) => (hashCode ^ symbol.GetHashCode()));
		}
		public override string ToString()
		{
			return string.Format("{0} -> {1}", _code.ToString(), string.Join(" ", _symbols.Select(symbol => symbol.ToString())));
		}
		public TProductionRuleCode Code
		{
			get
			{
				return _code;
			}
		}
		public IReadOnlyList<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> Symbols
		{
			get
			{
				return _symbols;
			}
		}

		private readonly TProductionRuleCode _code;
		private readonly IReadOnlyList<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> _symbols;
	}

	public static class ProductionRule
	{
		public static ProductionRule<TAtomCode, TProductionRuleCode> Create<TAtomCode, TProductionRuleCode>(TProductionRuleCode code, IEnumerable<ProductionRuleSymbol<TAtomCode, TProductionRuleCode>> symbols)
			where TAtomCode : struct
			where TProductionRuleCode : struct
		{
			return new ProductionRule<TAtomCode, TProductionRuleCode>(code, symbols);
		}
		public static ProductionRule<TAtomCode, TProductionRuleCode> Create<TAtomCode, TProductionRuleCode>(TProductionRuleCode code, params ProductionRuleSymbol<TAtomCode, TProductionRuleCode>[] symbols)
			where TAtomCode : struct
			where TProductionRuleCode : struct
		{
			return new ProductionRule<TAtomCode, TProductionRuleCode>(code, symbols);
		}
	}
}