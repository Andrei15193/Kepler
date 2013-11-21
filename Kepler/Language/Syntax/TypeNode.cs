using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class TypeNode
    {
        public TypeNode(ParsedNode<Lexicon> typeParsedNode)
        {
            if (typeParsedNode != null)
                if (typeParsedNode.Name == KeplerRuleSet.Type)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childNodes;

                    _name = string.Join(".", typeParsedNode[KeplerRuleSet.QualifiedIdentifier, 0][KeplerRuleSet.Name].Select(nameParsedNode => nameParsedNode.Atoms[0].Value));
                    if (typeParsedNode.TryGetChildNodeGroup(KeplerRuleSet.GenericParameters, out childNodes))
                        _genericParameters = childNodes[0][KeplerRuleSet.Type].Select(genericTypeNode => new TypeNode(genericTypeNode)).ToList();
                    if (typeParsedNode.TryGetChildNodeGroup(KeplerRuleSet.Array, out childNodes))
                        _arrayDimensions = childNodes.Select(arrayNode => arrayNode.Atoms.Count - 1).ToList();
                }
                else
                    throw new ArgumentException("Must be type node!", "typeParsedNode");
            else
                throw new ArgumentNullException("typeParsedNode");
        }

        public string ToCliTypeName()
        {
            StringBuilder cliTypeNameBuilder = new StringBuilder(_name);

            if (_genericParameters.Count > 0)
                cliTypeNameBuilder.Append("`")
                                  .Append(_genericParameters.Count)
                                  .Append("[")
                                  .Append(string.Join(", ", _genericParameters.Select(genericParameter => genericParameter.ToCliTypeName())))
                                  .Append("]");

            return cliTypeNameBuilder.ToString();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(_name);

            if (_genericParameters.Count > 0)
                stringBuilder.Append('<')
                             .Append(string.Join(", ", _genericParameters.Select(genericParameter => genericParameter.ToString())))
                             .Append('>');
            foreach (int arrayDimenison in _arrayDimensions)
            {
                stringBuilder.Append('[');
                for (int arrayDimensiounCount = 1; arrayDimensiounCount < arrayDimenison; arrayDimensiounCount++)
                    stringBuilder.Append(',');
                stringBuilder.Append(']');
            }

            return stringBuilder.ToString();
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string FullName
        {
            get
            {
                return ToString();
            }
        }

        public IReadOnlyList<int> ArrayDimensions
        {
            get
            {
                return _arrayDimensions;
            }
        }

        public IReadOnlyList<TypeNode> GenericParameters
        {
            get
            {
                return _genericParameters;
            }
        }

        private readonly string _name;
        private readonly IReadOnlyList<int> _arrayDimensions = new List<int>();
        private readonly IReadOnlyList<TypeNode> _genericParameters = new List<TypeNode>();
    }
}
