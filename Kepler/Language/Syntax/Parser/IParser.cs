using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public interface IParser
	{
		ProgramNode Parse(IReadOnlyList<ScannedAtom> atoms);
		IList<ProductionRule> ProductionRules
		{
			get;
		}
	}
}