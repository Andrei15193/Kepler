using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax.Parsers
{
	public interface IParser<TAtomCode, TProductionRuleCode>
		where TAtomCode : struct
		where TProductionRuleCode : struct
	{
		IParsedNode<TAtomCode, TProductionRuleCode> Parse(IReadOnlyList<ScannedAtom<TAtomCode>> atoms);
		TProductionRuleCode StartRuleCode
		{
			get;
			set;
		}
		ICollection<ProductionRule<TAtomCode, TProductionRuleCode>> ProductionRules
		{
			get;
		}
	}
}