using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax
{
	internal sealed class ParsedNode<TAtomCode, TProductionRuleCode>
		: IParsedNode<TAtomCode, TProductionRuleCode>
		where TAtomCode : struct
		where TProductionRuleCode : struct
	{
		public ParsedNode(TProductionRuleCode code)
		{
			_code = code;
		}

		#region IParsedNode<TAtomCode,TProductionRuleCode> Members
		public bool HasChilds(TProductionRuleCode code)
		{
			return _childs.ContainsKey(code);
		}
		public bool TryGetChilds(TProductionRuleCode childCode, out IReadOnlyList<IParsedNode<TAtomCode, TProductionRuleCode>> childs)
		{
			List<IParsedNode<TAtomCode, TProductionRuleCode>> childParsedNodes;

			if (_childs.TryGetValue(childCode, out childParsedNodes))
				childs = new ReadOnlyCollection<IParsedNode<TAtomCode, TProductionRuleCode>>(childParsedNodes);
			else
				childs = null;

			return (childs != null);
		}
		public IReadOnlyList<IParsedNode<TAtomCode, TProductionRuleCode>> this[TProductionRuleCode childCode]
		{
			get
			{
				return new ReadOnlyCollection<IParsedNode<TAtomCode, TProductionRuleCode>>(_childs[childCode]);
			}
		}
		public IParsedNode<TAtomCode, TProductionRuleCode> this[TProductionRuleCode childCode, int childIndex]
		{
			get
			{
				return _childs[childCode][childIndex];
			}
		}
		public TProductionRuleCode Code
		{
			get
			{
				return _code;
			}
		}
		public IReadOnlyList<ScannedAtom<TAtomCode>> Atoms
		{
			get
			{
				return new ReadOnlyCollection<ScannedAtom<TAtomCode>>(_atoms);
			}
		}
		public IReadOnlyCollection<TProductionRuleCode> ChildCodes
		{
			get
			{
				return _childs.Keys.ToList();
			}
		}
		public IReadOnlyCollection<IParsedNode<TAtomCode, TProductionRuleCode>> Childs
		{
			get
			{
				return _childs.Values.Cast<IParsedNode<TAtomCode, TProductionRuleCode>>().ToList();
			}
		}
		#endregion
		public void Add(IParsedNode<TAtomCode, TProductionRuleCode> child, bool ignoreAtoms = true)
		{
			if (child == null)
				throw new ArgumentNullException("parsedNode");

			if (!ignoreAtoms)
				_atoms.AddRange(child.Atoms);

			List<IParsedNode<TAtomCode, TProductionRuleCode>> childsWithSameCode;
			if (_childs.TryGetValue(child.Code, out childsWithSameCode))
				childsWithSameCode.Add(child);
			else
				_childs.Add(child.Code, new List<IParsedNode<TAtomCode, TProductionRuleCode>> { child });
		}
		public void Add(ScannedAtom<TAtomCode> atom)
		{
			_atoms.Add(atom);
		}

		private readonly TProductionRuleCode _code;
		private readonly List<ScannedAtom<TAtomCode>> _atoms = new List<ScannedAtom<TAtomCode>>();
		private readonly IDictionary<TProductionRuleCode, List<IParsedNode<TAtomCode, TProductionRuleCode>>> _childs = new Dictionary<TProductionRuleCode, List<IParsedNode<TAtomCode, TProductionRuleCode>>>();
	}
}