using System;
using System.IO;
using Andrei15193.Kepler.Language;
using Andrei15193.Kepler.Language.Lexic.Scanner;
using Andrei15193.Kepler.Language.Syntax.Parser;
namespace Andrei15193.Kepler
{
	internal static class Program
	{
		static private void Main(string[] args)
		{
			try
			{
				var scanResult = KeplerLanguage.GetScanner<DefaultScanner>().Scan(@"predicate Main
    try
        System::Console::Write(""Introduceti raza cercului: "").
        razaCerc : System::Int32 = System::Int32::Parse(System::Console::ReadLine()).
        PerimetruCerc(razaCerc).
        ArieCerc(razaCerc).
    catch
        System::Console::WriteLine(""Nu ati introdus un numar!"").
    end
end

predicate PerimetruCerc(razaCerc : System::Int32)
    System::Console::WriteLine(2 * razaCerc * 3.14).
end

predicate ArieCerc(razaCerc : System::Int32)
    System::Console::WriteLine(System::Math::Pow(razaCerc, 2) * 3.14).
end");
				KeplerLanguage.GetParser<LR1Parser>().Parse(scanResult);
				throw new NotImplementedException("Compiler not finished");
			}
			catch (Exception exception)
			{
			}


			//if (args.Length > 0)
			//{
			//	int errorCount = 0;
			//	Compiler compiler = new Compiler();

			//	using (StreamWriter errorStreamWriter = new StreamWriter(File.Open("errors.txt", FileMode.Create)))
			//	{
			//		foreach (string sourceFile in args)
			//			try
			//			{
			//				new CilGenerator().Generate(sourceFile.Substring(0, sourceFile.LastIndexOf('.')), compiler.Parse(string.Join(Environment.NewLine, File.ReadAllLines(sourceFile))));
			//			}
			//			catch (Exception exception)
			//			{
			//				errorCount++;
			//				errorStreamWriter.WriteLine("Error in source: " + sourceFile);
			//				errorStreamWriter.WriteLine("Reason: " + exception.Message);
			//				errorStreamWriter.WriteLine();
			//			}
			//		errorStreamWriter.WriteLine("Number of errors: {0}.", errorCount);
			//	}
			//	Console.WriteLine("Lexical analysis finished!");
			//}
			//else
			//	Console.WriteLine("No source file(s) provided!");
		}
	}
}
