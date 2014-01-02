﻿using System;
using System.IO;
namespace Andrei15193.Kepler
{
	internal static class Program
	{
		static private void Main(string[] args)
		{
			if (args.Length > 0)
			{
				int errorCount = 0;
				Compiler compiler = new Compiler();

				using (StreamWriter errorStreamWriter = new StreamWriter(File.Open("errors.txt", FileMode.Create)))
				{
					foreach (string sourceFile in args)
						try
						{
							new CilGenerator().Generate(sourceFile.Substring(0, sourceFile.LastIndexOf('.')), compiler.Parse(string.Join(Environment.NewLine, File.ReadAllLines(sourceFile))));
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
