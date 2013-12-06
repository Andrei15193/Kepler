using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class TryCatchFinallyStatementNode
        : StatementNode
    {
        internal TryCatchFinallyStatementNode(ParsedNode<Lexicon> tryCatchFinallyParsedNode)
        {
            if (tryCatchFinallyParsedNode != null)
                if (tryCatchFinallyParsedNode.Name == KeplerRuleSet.TryCatchFinallyStatement)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childNodes;

                    _try = new BodyNode(tryCatchFinallyParsedNode[KeplerRuleSet.Body, 0]);
                    if (tryCatchFinallyParsedNode.TryGetChildNodeGroup(KeplerRuleSet.CatchStatement, out childNodes))
                    {
                        IReadOnlyList<ParsedNode<Lexicon>> catchChildNodes;

                        if (childNodes[0].TryGetChildNodeGroup(KeplerRuleSet.CatchAllStatement, out catchChildNodes))
                            _defaultCatch = new BodyNode(catchChildNodes[0][KeplerRuleSet.Body, 0]);
                        if (childNodes[0].TryGetChildNodeGroup(KeplerRuleSet.CatchBlockStatement, out catchChildNodes))
                            _catchBlocks = catchChildNodes.Select(catchChildNode => new CatchBlockNode(catchChildNode)).ToList();
                        else
                            _catchBlocks = new CatchBlockNode[0];
                    }
                    if (tryCatchFinallyParsedNode.TryGetChildNodeGroup(KeplerRuleSet.FinallyStatement, out childNodes))
                        _finally = new BodyNode(childNodes[0][KeplerRuleSet.Body, 0]);
                }
                else
                    throw new ArgumentException("Expected try catch finally statement node!", "tryCatchFinallyParsedNode");
            else
                throw new ArgumentNullException("tryCatchFinallyParsedNode");
        }

        public override StatementNodeType StatementType
        {
            get
            {
                return StatementNodeType.TryCatchFinally;
            }
        }

        public BodyNode Try
        {
            get
            {
                return _try;
            }
        }

        public BodyNode DefaultCatch
        {
            get
            {
                return _defaultCatch;
            }
        }

        public BodyNode Finally
        {
            get
            {
                return _finally;
            }
        }

        public IReadOnlyList<CatchBlockNode> CatchBlocks
        {
            get
            {
                return _catchBlocks;
            }
        }

        private readonly BodyNode _try;
        private readonly BodyNode _finally = null;
        private readonly BodyNode _defaultCatch = null;
        private readonly IReadOnlyList<CatchBlockNode> _catchBlocks;
    }
}
