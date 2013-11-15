using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class ParsedNode<TCode>
        where TCode : struct
    {
        public ParsedNode(string name, ParsedNode<TCode> parent = null)
        {
            if (name != null)
            {
                _name = name.Trim();
                if (_name.Length > 0)
                    _parent = parent;
                else
                    throw new ArgumentException("Cannot be emty!", "name");
            }
            else
                throw new ArgumentNullException("name");
        }

        public bool HasChildNodeGroup(string childNodeGroupName)
        {
            return (childNodeGroupName != null && _childNodes.ContainsKey(childNodeGroupName));
        }

        public bool TryGetChildNodeGroup(string childNodeGroupName, out IReadOnlyList<ParsedNode<TCode>> childNodeGroup)
        {
            IList<ParsedNode<TCode>> childNodeGroupList;

            if (childNodeGroupName == null
                || !_childNodes.TryGetValue(childNodeGroupName, out childNodeGroupList))
                childNodeGroup = null;
            else
                childNodeGroup = new ReadOnlyCollection<ParsedNode<TCode>>(childNodeGroupList);

            return (childNodeGroup != null);
        }

        public IReadOnlyList<ParsedNode<TCode>> this[string childNodeGroupName]
        {
            get
            {
                if (childNodeGroupName != null)
                    return new ReadOnlyCollection<ParsedNode<TCode>>(_childNodes[childNodeGroupName]);
                else
                    throw new ArgumentNullException("childNodeGroupName");
            }
        }

        public ParsedNode<TCode> this[string childNodeGroupName, int childNodeIndex]
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

        public ParsedNode<TCode> Parent
        {
            get
            {
                return _parent;
            }
            internal set
            {
                _parent = value;
            }
        }

        public IReadOnlyList<ParsedNode<TCode>> ChildNodes
        {
            get
            {
                return new ReadOnlyCollection<ParsedNode<TCode>>(_childNodes.SelectMany(childNodes => childNodes.Value).ToList());
            }
        }

        public IReadOnlyList<ScannedAtom<TCode>> Atoms
        {
            get
            {
                return new ReadOnlyCollection<ScannedAtom<TCode>>(_atoms);
            }
        }

        public IReadOnlyList<string> ChildNodeGroups
        {
            get
            {
                return _childNodes.Keys.ToList();
            }
        }

        public bool HasParent
        {
            get
            {
                return (_parent != null);
            }
        }

        internal void Add(ParsedNode<TCode> childNode, bool appendAtoms = true)
        {
            if (childNode != null)
            {
                IList<ParsedNode<TCode>> childNodesWithSameName;

                if (_childNodes.TryGetValue(childNode.Name, out childNodesWithSameName))
                    childNodesWithSameName.Add(childNode);
                else
                    _childNodes.Add(childNode.Name, new List<ParsedNode<TCode>> { childNode });
                if (appendAtoms)
                    _atoms.AddRange(childNode.Atoms);
            }
            else
                throw new ArgumentNullException("childNode");
        }

        internal void Add(ScannedAtom<TCode> atom)
        {
            if (atom != null)
                _atoms.Add(atom);
            else
                throw new ArgumentNullException("atom");
        }

        internal void Add(IEnumerable<ScannedAtom<TCode>> atoms)
        {
            if (atoms != null)
                if (atoms.Count(atom => atom == null) == 0)
                    _atoms.AddRange(atoms);
                else
                    throw new ArgumentException("Cannot contain null values!", "atoms");
            else
                throw new ArgumentNullException("atoms");
        }

        internal void Add(ScannedAtom<TCode> atom, params ScannedAtom<TCode>[] atoms)
        {
            Add(atom);
            Add((IEnumerable<ScannedAtom<TCode>>)atoms);
        }

        private ParsedNode<TCode> _parent;
        private readonly string _name;
        private readonly List<ScannedAtom<TCode>> _atoms = new List<ScannedAtom<TCode>>();
        private readonly IDictionary<string, IList<ParsedNode<TCode>>> _childNodes = new SortedDictionary<string, IList<ParsedNode<TCode>>>();
    }
}
