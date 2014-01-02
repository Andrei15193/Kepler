using System;
using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public class LRParser
		: IParser
	{
		#region IParser Members
		public ProgramNode Parse(IReadOnlyList<ScannedAtom> atoms)
		{
			throw new NotImplementedException();
		}
		public IList<ProductionRule> ProductionRules
		{
			get
			{
				return _productionRules;
			}
		}
		#endregion

		private readonly IList<ProductionRule> _productionRules = new List<ProductionRule>();
	}
}