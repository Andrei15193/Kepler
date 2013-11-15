namespace Andrei15193.Kepler.Language.Lexis
{
    public interface ILexicalAnalyser<TCode>
        where TCode : struct
    {
        ScanResult<TCode> Scan(string text, ILanguage<TCode> language);
    }
}
