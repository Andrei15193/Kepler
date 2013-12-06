using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public class QualifiedIdentifierNode
    {
        internal QualifiedIdentifierNode(ParsedNode<Lexicon> qualifiedIdentifierParsedNode)
        {
            if (qualifiedIdentifierParsedNode != null)
                if (qualifiedIdentifierParsedNode.Name == KeplerRuleSet.QualifiedIdentifier)
                {
                    _nameParts = qualifiedIdentifierParsedNode[KeplerRuleSet.Name].Select(nameParsedNode => nameParsedNode.Atoms[0].Value).ToList();
                    _cliName = string.Join(".", _nameParts);
                }
                else
                    throw new ArgumentException("The provided instance must be a qualified identifier parsed node!", "qualifiedIdentifierParsedNode");
            else
                throw new ArgumentNullException("qualifiedIdentifierParsedNode");
        }

        internal QualifiedIdentifierNode(string name)
        {
            if (name != null)
                if (Regex.IsMatch(name, "^[_a-zA-Z][_a-zA-Z0-9]*$", RegexOptions.Compiled))
                {
                    _nameParts = new[] { name };
                    _cliName = string.Join(".", _nameParts);
                }
                else
                    throw new ArgumentException("Expected identifier!", "name");
            else
                throw new ArgumentNullException("name");
        }

        public override string ToString()
        {
            return _cliName;
        }

        public string ToString(string separator)
        {
            if (separator == ".")
                return _cliName;
            else
                return string.Join(separator ?? string.Empty, _nameParts);
        }

        public IReadOnlyList<string> NameParts
        {
            get
            {
                return _nameParts;
            }
        }

        private readonly string _cliName;
        private readonly IReadOnlyList<string> _nameParts;
    }
}
