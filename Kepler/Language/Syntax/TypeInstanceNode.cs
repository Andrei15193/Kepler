using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class TypeInstanceNode
    {
        internal TypeInstanceNode(ParsedNode<Lexicon> typeInstanceParsedNode)
        {
            if (typeInstanceParsedNode != null)
                if (typeInstanceParsedNode.Name == KeplerRuleSet.TypeInstance)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childNodes;

                    _typeName = new QualifiedIdentifierNode(typeInstanceParsedNode[KeplerRuleSet.QualifiedIdentifier, 0]);
                    if (typeInstanceParsedNode.TryGetChildNodeGroup(KeplerRuleSet.GenericParameters, out childNodes))
                        _genericParameters = childNodes[0][KeplerRuleSet.Type].Select(genericTypeNode => new TypeNode(genericTypeNode)).ToList();
                    else
                        _genericParameters = new TypeNode[0];
                    _arrayBounds = typeInstanceParsedNode[KeplerRuleSet.BoundedArray].Select(arraybound => arraybound.Atoms
                                                                                                                     .Where(atom => atom.Code == Lexicon.IntegerNumericConstant)
                                                                                                                     .Select(atom => int.Parse(atom.Value))
                                                                                                                     .ToList())
                                                                                     .ToList();
                }
                else
                    throw new ArgumentException("Expected type instance node!", "typeParsedNode");
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
            cliTypeNameBuilder.Append(string.Join("", _arrayBounds.Select(arrayBound => "[" + new string(',', arrayBound.Count - 1) + "]")));

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

        public QualifiedIdentifierNode TypeName
        {
            get
            {
                return _typeName;
            }
        }

        public IReadOnlyList<TypeNode> GenericParameters
        {
            get
            {
                return _genericParameters;
            }
        }

        public IReadOnlyList<IReadOnlyList<int>> ArrayBounds
        {
            get
            {
                return _arrayBounds;
            }
        }

        private readonly QualifiedIdentifierNode _typeName;
        private readonly IReadOnlyList<TypeNode> _genericParameters;
        private readonly IReadOnlyList<IReadOnlyList<int>> _arrayBounds;
    }
}
