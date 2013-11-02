
namespace Andrei15193.Kepler.Language.Lexis
{
    public interface ILexicalAnalyser<TCode>
        where TCode : struct
    {
        LexicalAnalysisResult<TCode> Analyse(string text, ILanguage<TCode> language);
    }
}
