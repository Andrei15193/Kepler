using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class ParseNode<TCode>
        where TCode : struct
    {
        public ParseNode(string name, ParseNode<TCode> parent = null)
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

        public ParseNode<TCode> GetChildNode(string name)
        {
            if (name != null)
                return _childNodes[name][0];
            else
                throw new ArgumentNullException("name");
        }

        public IReadOnlyList<ParseNode<TCode>> GetChildNodes(string name)
        {
            if (name != null)
                return new ReadOnlyCollection<ParseNode<TCode>>(_childNodes[name]);
            else
                throw new ArgumentNullException("name");
        }

        public IReadOnlyList<ParseNode<TCode>> this[string name]
        {
            get
            {
                return GetChildNodes(name);
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ParseNode<TCode> Parent
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

        public IReadOnlyList<ParseNode<TCode>> ChildNodes
        {
            get
            {
                return new ReadOnlyCollection<ParseNode<TCode>>(_childNodes.SelectMany(childNodes => childNodes.Value).ToList());
            }
        }

        public IReadOnlyList<ScannedAtom<TCode>> Atoms
        {
            get
            {
                return new ReadOnlyCollection<ScannedAtom<TCode>>(_atoms);
            }
        }

        public bool HasParent
        {
            get
            {
                return (_parent != null);
            }
        }

        internal void Add(ParseNode<TCode> childNode)
        {
            if (childNode != null)
            {
                IList<ParseNode<TCode>> childNodesWithSameName;

                if (_childNodes.TryGetValue(childNode.Name, out childNodesWithSameName))
                    childNodesWithSameName.Add(childNode);
                else
                    _childNodes.Add(childNode.Name, new List<ParseNode<TCode>> { childNode });
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

        private ParseNode<TCode> _parent;
        private readonly string _name;
        private readonly List<ScannedAtom<TCode>> _atoms = new List<ScannedAtom<TCode>>();
        private readonly IDictionary<string, IList<ParseNode<TCode>>> _childNodes = new SortedDictionary<string, IList<ParseNode<TCode>>>();
    }
}
