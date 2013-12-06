using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    public sealed class BodyNode
    {
        internal BodyNode(ParsedNode<Lexicon> bodyParsedNode)
        {
            if (bodyParsedNode != null)
                if (bodyParsedNode.Name == KeplerRuleSet.Body)
                {
                    IReadOnlyList<ParsedNode<Lexicon>> childParsedNodes;
                    List<StatementNode> statements = new List<StatementNode>();

                    if (bodyParsedNode.TryGetChildNodeGroup(KeplerRuleSet.Statement, out childParsedNodes))
                        foreach (ParsedNode<Lexicon> statementParsedNode in childParsedNodes)
                            switch (statementParsedNode.ChildNodeGroups[0])
                            {
                                case KeplerRuleSet.WhenStatement:
                                    statements.Add(new WhenStatementNode(statementParsedNode[KeplerRuleSet.WhenStatement, 0]));
                                    break;
                                case KeplerRuleSet.WhileStatement:
                                    statements.Add(new WhileStatementNode(statementParsedNode[KeplerRuleSet.WhileStatement, 0]));
                                    break;
                                case KeplerRuleSet.TryCatchFinallyStatement:
                                    statements.Add(new TryCatchFinallyStatementNode(statementParsedNode[KeplerRuleSet.TryCatchFinallyStatement, 0]));
                                    break;
                                case KeplerRuleSet.ThrowStatement:
                                    statements.Add(new ThrowStatementNode(statementParsedNode[KeplerRuleSet.ThrowStatement, 0]));
                                    break;
                                case KeplerRuleSet.VariableDeclarationStatement:
                                    statements.Add(new VariableDeclarationStatementNode(statementParsedNode[KeplerRuleSet.VariableDeclarationStatement, 0]));
                                    break;
                                case KeplerRuleSet.VariableAssignmentStatement:
                                    statements.Add(new VariableAssignmentStatementNode(statementParsedNode[KeplerRuleSet.VariableAssignmentStatement, 0]));
                                    break;
                                case KeplerRuleSet.FunctionCall:
                                    statements.Add(new FunctionCallStatementNode(statementParsedNode[KeplerRuleSet.FunctionCall, 0]));
                                    break;
                                case KeplerRuleSet.ExitStatement:
                                    statements.Add(new ExitStatementNode(statementParsedNode[KeplerRuleSet.ExitStatement, 0]));
                                    break;
                            }
                    _statements = statements;
                }
                else
                    throw new ArgumentException("Expected body node!", "bodyParsedNode");
            else
                throw new ArgumentNullException("bodyParsedNode");
        }

        internal BodyNode(IEnumerable<StatementNode> statements)
        {
            if (statements != null)
                _statements = statements.ToList();
            else
                throw new ArgumentNullException("statements");
        }

        internal BodyNode(params StatementNode[] statements)
            : this((IEnumerable<StatementNode>)statements)
        {
        }

        public IReadOnlyList<StatementNode> Statements
        {
            get
            {
                return _statements;
            }
        }

        private IReadOnlyList<StatementNode> _statements;
    }
}
