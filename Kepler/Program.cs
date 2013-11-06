using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Andrei15193.Kepler.Language;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax.NonTerminalSymbols;

namespace Andrei15193.Kepler
{
    internal static class Program
    {
        static private void Main(string[] args)
        {
            //var builder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("test"), AssemblyBuilderAccess.Save);
            //var moduleBuilder = builder.DefineDynamicModule("test", "test.dll", true);
            //var typeBuilder = moduleBuilder.DefineType("testClass", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, typeof(object));
            //var methodBuilder = typeBuilder.DefineMethod("Invoke", MethodAttributes.Public, CallingConventions.HasThis, typeof(string), new[] { typeof(string) });
            //Expression.Lambda(Expression.Block(typeof(string), Expression.Constant("ret", typeof(string))), "Invoke", new[] { Expression.Parameter(typeof(string), "b") }).CompileToMethod(methodBuilder);
            //// methodBuilder.CreateMethodBody(new byte[0], 0);
            ////var methIlGen = methodBuilder.GetILGenerator();
            ////methIlGen.Emit(OpCodes.Ldstr, "return");

            ////methIlGen.Emit(OpCodes.Ret);
            //////var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, Type.EmptyTypes);
            //////var ilGenerator = constructorBuilder.GetILGenerator();
            //////ilGenerator.Emit(OpCodes.Ldarg_0);
            //////ilGenerator.Emit(OpCodes.Ldstr, "Invoke");
            //////ilGenerator.Emit(OpCodes.Call, typeof(MulticastDelegate).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First(constructorInfo => constructorInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).SequenceEqual(new[] { typeof(object), typeof(string) })));
            //typeBuilder.CreateType();
            //builder.Save("test.dll");


            //Environment.Exit(0);

            //var atoms = new[]
            //{
            //    new ScannedAtom<Lexicon>(Lexicon.Identifier, 0, 0, "a"),
            //    new ScannedAtom<Lexicon>(Lexicon.LessThan, 1, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.Identifier, 2, 0, "b"),
            //    new ScannedAtom<Lexicon>(Lexicon.Comma, 3, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.Identifier, 4, 0, "c"),
            //    new ScannedAtom<Lexicon>(Lexicon.LessThan, 5, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.Identifier, 6, 0, "d"),
            //    new ScannedAtom<Lexicon>(Lexicon.GreaterThan, 7, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.Comma, 8, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.Identifier, 9, 0, "e"),
            //    new ScannedAtom<Lexicon>(Lexicon.GreaterThan, 10, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.OpeningSquareParenthesis, 11, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.IntegerNumericConstant, 12, 0, "1"),
            //    new ScannedAtom<Lexicon>(Lexicon.Comma, 12, 20),
            //    new ScannedAtom<Lexicon>(Lexicon.IntegerNumericConstant, 12, 0, "2"),
            //    new ScannedAtom<Lexicon>(Lexicon.ClosingSquareParenthesis, 12, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.OpeningSquareParenthesis, 13, 0),
            //    new ScannedAtom<Lexicon>(Lexicon.IntegerNumericConstant, 12, 0, "3"),
            //    new ScannedAtom<Lexicon>(Lexicon.ClosingSquareParenthesis, 14, 0)
            //};

            var atoms = new[]
            {
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 0, 0, "a"),
                new ScannedAtom<Lexicon>(Lexicon.Plus, 1, 0),
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 2, 0, "b"),
                new ScannedAtom<Lexicon>(Lexicon.Minus, 1, 0),
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 2, 0, "c"),
                new ScannedAtom<Lexicon>(Lexicon.Star, 2, 0),
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 2, 0, "d"),
                new ScannedAtom<Lexicon>(Lexicon.Plus, 2, 0),
                new ScannedAtom<Lexicon>(Lexicon.Identifier, 2, 0, "e")
            };

            var x = new ExpressionSymbol(atoms, new RegexLanguage<Lexicon>());

            if (args.Length > 0)
            {
                int errorCount = 0;
                Compiler<Lexicon> compiler = new Compiler<Lexicon>(new RegexLanguage<Lexicon>());

                using (StreamWriter errorStreamWriter = new StreamWriter(File.Open("errors.txt", FileMode.Create)))
                {
                    foreach (string sourceFile in args)
                        try
                        {
                            string sourceCode = string.Join(Environment.NewLine, File.ReadAllLines(sourceFile));
                            LexicalAnalysisResult<Lexicon> lexicalAnalysisResult = compiler.LexicallyAnalyse(sourceCode);

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
