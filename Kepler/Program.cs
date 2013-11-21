using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Andrei15193.Kepler.AbstractCore;
using Andrei15193.Kepler.Language.Lexis;
using Andrei15193.Kepler.Language.Syntax;

namespace Andrei15193.Kepler
{
    internal static class Program
    {
        static private void Main(string[] args)
        {
            if (args.Length > 0)
            {
                int errorCount = 0;
                Compiler compiler = new Compiler(new KeplerRuleSet());

                using (StreamWriter errorStreamWriter = new StreamWriter(File.Open("errors.txt", FileMode.Create)))
                {
                    foreach (string sourceFile in args)
                        try
                        {
                            string sourceCode = string.Join(Environment.NewLine, File.ReadAllLines(sourceFile));
                            ScanResult<Lexicon> scanResult = compiler.Scan(sourceCode);

                            using (StreamWriter streamWriter = new StreamWriter(File.Open(sourceFile + ".fip.txt", FileMode.Create)))
                                foreach (var scannedAtom in scanResult.ScannedAtoms)
                                    streamWriter.WriteLine("{0},{1},{2}", scannedAtom.Code.ToString(), (uint)scannedAtom.Code, scannedAtom.Value);

                            using (StreamWriter streamWriter = new StreamWriter(File.Open(sourceFile + ".tsi.txt", FileMode.Create)))
                                foreach (string identifier in scanResult.Identifiers.Values)
                                    streamWriter.WriteLine(identifier);

                            using (StreamWriter streamWriter = new StreamWriter(File.Open(sourceFile + ".tsc.txt", FileMode.Create)))
                                foreach (string constant in scanResult.Constants.Values)
                                    streamWriter.WriteLine(constant);

                            using (Stream stream = File.Open(sourceFile + ".BinaryLexicalAnalysis", FileMode.Create))
                                new BinaryFormatter().Serialize(stream, scanResult);

                            Queue<ParsedNode<Lexicon>> nodesToVisit = new Queue<ParsedNode<Lexicon>>();
                            nodesToVisit.Enqueue(compiler.Parse(scanResult));

                            if (nodesToVisit.Peek() != null)
                            {
                                new CilGenerator().Generate(sourceFile, sourceFile + ".dll", nodesToVisit.Peek(), new CilGeneratorSettings
                                    {
                                        OutputType = OutputType.DynamicLinkLibrary
                                    });

                                using (StreamWriter writer = new StreamWriter(File.Open(sourceFile + ".syntax.txt", FileMode.Create)))
                                    while (nodesToVisit.Count > 0)
                                    {
                                        ParsedNode<Lexicon> currentNode = nodesToVisit.Dequeue();

                                        writer.WriteLine(currentNode.Name);
                                        writer.WriteLine();
                                        writer.WriteLine("Atom list ({0} atoms):", currentNode.Atoms.Count);
                                        writer.WriteLine(string.Join(" ", currentNode.Atoms.Select(atom => atom.HasValue ? atom.Value : atom.Code.ToString())));
                                        writer.WriteLine();
                                        foreach (string childNodeGroupName in currentNode.ChildNodeGroups)
                                        {
                                            writer.WriteLine(" > " + childNodeGroupName);
                                            foreach (ParsedNode<Lexicon> childNode in currentNode[childNodeGroupName])
                                                nodesToVisit.Enqueue(childNode);
                                        }
                                        writer.WriteLine("Child count: " + currentNode.ChildNodes.Count);
                                        writer.WriteLine(new string('-', 80));
                                    }
                            }
                            else
                                errorStreamWriter.WriteLine(sourceFile + " is not syntactically correct!");
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
