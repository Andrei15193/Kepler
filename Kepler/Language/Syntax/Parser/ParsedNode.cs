using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Language.Lexic;
namespace Andrei15193.Kepler.Language.Syntax.Parser
{
	public sealed class ParsedNode
	{
		public ParsedNode(string name)
		{
			if (name != null)
				throw new ArgumentNullException("name");
			_name = name.Trim();
			if (_name.Length > 0)
				throw new ArgumentException("Cannot be emty or whitespace only!", "name");
		}

		public bool HasChildNodeGroup(string childNodeGroupName)
		{
			return (childNodeGroupName != null && _childNodes.ContainsKey(childNodeGroupName));
		}
		public bool TryGetChildNodeGroup(string childNodeGroupName, out IReadOnlyList<ParsedNode> childNodeGroup)
		{
			IList<ParsedNode> childNodeGroupList;

			if (childNodeGroupName == null
				|| !_childNodes.TryGetValue(childNodeGroupName, out childNodeGroupList))
				childNodeGroup = null;
			else
				childNodeGroup = new ReadOnlyCollection<ParsedNode>(childNodeGroupList);

			return (childNodeGroup != null);
		}
		public IReadOnlyList<ParsedNode> this[string childNodeGroupName]
		{
			get
			{
				if (childNodeGroupName != null)
					return new ReadOnlyCollection<ParsedNode>(_childNodes[childNodeGroupName]);
				else
					throw new ArgumentNullException("childNodeGroupName");
			}
		}
		public ParsedNode this[string childNodeGroupName, int childNodeIndex]
		{
			get
			{
				return this[childNodeGroupName][childNodeIndex];
			}
		}
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public IReadOnlyList<ParsedNode> ChildNodes
		{
			get
			{
				return new ReadOnlyCollection<ParsedNode>(_childNodes.SelectMany(childNodes => childNodes.Value).ToList());
			}
		}
		public IReadOnlyList<ScannedAtom> Atoms
		{
			get
			{
				return new ReadOnlyCollection<ScannedAtom>(_atoms);
			}
		}
		public IReadOnlyList<string> ChildNodeGroups
		{
			get
			{
				return _childNodes.Keys.ToList();
			}
		}

		internal void Add(ParsedNode childNode, bool appendAtoms = true)
		{
			if (childNode != null)
			{
				IList<ParsedNode> childNodesWithSameName;

				if (_childNodes.TryGetValue(childNode.Name, out childNodesWithSameName))
					childNodesWithSameName.Add(childNode);
				else
					_childNodes.Add(childNode.Name, new List<ParsedNode> { childNode });
				if (appendAtoms)
					_atoms.AddRange(childNode.Atoms);
			}
			else
				throw new ArgumentNullException("childNode");
		}
		internal void Add(ScannedAtom atom)
		{
			if (atom != null)
				_atoms.Add(atom);
			else
				throw new ArgumentNullException("atom");
		}
		internal void Add(IEnumerable<ScannedAtom> atoms)
		{
			if (atoms != null)
				if (atoms.Count(atom => atom == null) == 0)
					_atoms.AddRange(atoms);
				else
					throw new ArgumentException("Cannot contain null values!", "atoms");
			else
				throw new ArgumentNullException("atoms");
		}
		internal void Add(ScannedAtom atom, params ScannedAtom[] atoms)
		{
			Add(atom);
			Add((IEnumerable<ScannedAtom>)atoms);
		}

		private readonly string _name;
		private readonly List<ScannedAtom> _atoms = new List<ScannedAtom>();
		private readonly IDictionary<string, IList<ParsedNode>> _childNodes = new SortedDictionary<string, IList<ParsedNode>>();
	}
}