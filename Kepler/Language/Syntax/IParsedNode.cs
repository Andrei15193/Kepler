using System.Collections.Generic;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax
{
	public interface IParsedNode<TAtomCode, TProductionRuleCode>
		where TAtomCode : struct
		where TProductionRuleCode : struct
	{
		bool HasChilds(TProductionRuleCode code);
		bool TryGetChilds(TProductionRuleCode childCode, out IReadOnlyList<IParsedNode<TAtomCode, TProductionRuleCode>> childs);
		IReadOnlyList<IParsedNode<TAtomCode, TProductionRuleCode>> this[TProductionRuleCode childCode]
		{
			get;
		}
		IParsedNode<TAtomCode, TProductionRuleCode> this[TProductionRuleCode childCode, int childIndex]
		{
			get;
		}
		TProductionRuleCode Code
		{
			get;
		}
		IReadOnlyList<ScannedAtom<TAtomCode>> Atoms
		{
			get;
		}
		IReadOnlyCollection<TProductionRuleCode> ChildCodes
		{
			get;
		}
		IReadOnlyCollection<IParsedNode<TAtomCode, TProductionRuleCode>> Childs
		{
			get;
		}
	}
}