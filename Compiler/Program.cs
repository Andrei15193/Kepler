namespace Andrei15193.Kepler.Compiler
{
    using Andrei15193.Kepler.Compiler.Regex;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    internal static class Program
    {
        static private void Main(string[] args)
        {
            if (args.Length > 0)
            {
                int errorCount = 0;
                Compiler<LanguageSpecification> compiler = new Compiler<LanguageSpecification>(new RegexLanguage<LanguageSpecification>());

                using (StreamWriter errorStreamWriter = new StreamWriter(File.Open("errors.txt", FileMode.Create)))
                {
                    foreach (string sourceFile in args)
                        try
                        {
                            string sourceCode = string.Join(Environment.NewLine, File.ReadAllLines(sourceFile));
                            Compiler<LanguageSpecification>.LexicalAnalysisResult lexicalAnalysisResult = compiler.LexicallyAnalyze(sourceCode);
                          
                            using (StreamWriter streamWriter = new StreamWriter(File.Open(sourceFile + ".fip.txt", FileMode.Create)))
                                foreach (var scannedAtom in lexicalAnalysisResult.ScannedAtoms)
                                    streamWriter.WriteLine("{0},{1},{2}", scannedAtom.Code.ToString(), (uint)scannedAtom.Code, scannedAtom.Value);

                            using (StreamWriter streamWriter = new StreamWriter(File.Open(sourceFile + ".tsi.txt", FileMode.Create)))
                                foreach (string identifier in lexicalAnalysisResult.Identifiers.Values)
                                    streamWriter.WriteLine(identifier);

                            using (StreamWriter streamWriter = new StreamWriter(File.Open(sourceFile + ".tsc.txt", FileMode.Create)))
                                foreach (string constant in lexicalAnalysisResult.Constants.Values)
                                    streamWriter.WriteLine(constant);

                            using (Stream stream = File.Open(sourceFile + ".BinaryLexicalAnalysis", FileMode.Create))
                                new BinaryFormatter().Serialize(stream, lexicalAnalysisResult);

                        }
                        catch (Exception exception)
                        {
                            errorCount++;
                            errorStreamWriter.WriteLine("Error in source: " + sourceFile);
                            errorStreamWriter.WriteLine("Reason: " + exception.Message);
                            errorStreamWriter.WriteLine();
                        }
                    errorStreamWriter.WriteLine("Number of errors: {0}.", errorCount);
                }
                Console.WriteLine("Lexical analysis finished!");
            }
            else
                Console.WriteLine("No source file(s) provided!");
        }
    }
}
