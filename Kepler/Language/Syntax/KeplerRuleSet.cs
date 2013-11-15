using Andrei15193.Kepler.Language.Lexis;

namespace Andrei15193.Kepler.Language.Syntax
{
    internal sealed class KeplerRuleSet
        : RuleSet<Lexicon>
    {
        public KeplerRuleSet(ILanguage<Lexicon> language)
            : base(language)
        {
            Add("statement", RuleNode.RuleSequence("exit"), RuleNode.Atom(";"));
            Add("exit", RuleNode.Atom("stop"));
            Add("exit", RuleNode.Atom("skip"));
        }
    }
}
