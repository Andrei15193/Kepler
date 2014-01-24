using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public interface IParser
	{
		ParsedNode Parse(IReadOnlyList<ScannedAtom> atoms);
		ProductionRuleCode StartRule
		{
			get;
			set;
		}
		IList<ProductionRule> ProductionRules
		{
			get;
		}
	}
}