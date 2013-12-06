﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class TypeNode
    {
        internal TypeNode(ParsedNode<Lexicon> typeParsedNode)
        {
            if (typeParsedNode != null)
                if (typeParsedNode.Name == KeplerRuleSet.Type)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childNodes;

                    _typeName = new QualifiedIdentifierNode(typeParsedNode[KeplerRuleSet.QualifiedIdentifier, 0]);
                    if (typeParsedNode.TryGetChildNodeGroup(KeplerRuleSet.GenericParameters, out childNodes))
                        _genericParameters = childNodes[0][KeplerRuleSet.Type].Select(genericTypeNode => new TypeNode(genericTypeNode)).ToList();
                    if (typeParsedNode.TryGetChildNodeGroup(KeplerRuleSet.Array, out childNodes))
                        _arrayDimensions = childNodes.Select(arrayNode => arrayNode.Atoms.Count - 1).ToList();
                }
                else
                    throw new ArgumentException("Expected type node!", "typeParsedNode");
            else
                throw new ArgumentNullException("typeParsedNode");
        }

        public string GetCliTypeName()
        {
            StringBuilder cliTypeNameBuilder = new StringBuilder(_typeName.ToString());

            if (_genericParameters.Count > 0)
                cliTypeNameBuilder.Append("`")
                                  .Append(_genericParameters.Count)
                                  .Append("[")
                                  .Append(string.Join(", ", _genericParameters.Select(genericParameter => genericParameter.GetCliTypeName())))
                                  .Append("]");
            cliTypeNameBuilder.Append(string.Join("", _arrayDimensions.Select(arrayDimension => "[" + new string(',', arrayDimension - 1) + "]")));

            return cliTypeNameBuilder.ToString();
        }

        public string GetCliElementTypeName()
        {
            StringBuilder cliTypeNameBuilder = new StringBuilder(_typeName.ToString());

            if (_genericParameters.Count > 0)
                cliTypeNameBuilder.Append("`")
                                  .Append(_genericParameters.Count)
                                  .Append("[")
                                  .Append(string.Join(", ", _genericParameters.Select(genericParameter => genericParameter.GetCliTypeName())))
                                  .Append("]");

            return cliTypeNameBuilder.ToString();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(_typeName.ToString());

            if (_genericParameters.Count > 0)
                stringBuilder.Append('<')
                             .Append(string.Join(", ", _genericParameters.Select(genericParameter => genericParameter.ToString())))
                             .Append('>');
            foreach (int arrayDimenison in _arrayDimensions)
                stringBuilder.Append('[')
                             .Append(new string(',', arrayDimenison - 1))
                             .Append(']');

            return stringBuilder.ToString();
        }

        public QualifiedIdentifierNode TypeName
        {
            get
            {
                return _typeName;
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

        private readonly QualifiedIdentifierNode _typeName;
        private readonly IReadOnlyList<int> _arrayDimensions = new List<int>();
        private readonly IReadOnlyList<TypeNode> _genericParameters = new List<TypeNode>();
    }
}
