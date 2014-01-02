using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Andrei15193.Kepler.Language.Lexic.Scanner
{
	public sealed class DefaultScanner
		: IScanner
	{
		#region IScanner Members
		public IReadOnlyList<ScannedAtom> Scan(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			int currentIndex = 0;
			uint line = 1, column = 1;
			bool? isPreviousSeparator = null;
			StringBuilder unknownSymbolBuilder = new StringBuilder();
			List<ScannedAtom> scannedAtoms = new List<ScannedAtom>();
			IList<ArgumentException> unknownSymbolExceptons = new List<ArgumentException>();
			IReadOnlyCollection<AtomSpecification> _separatorSpecifications = _atomSpecifications.Where(atomSpecification => atomSpecification.IsSeparator)
																								 .ToList();
			while (currentIndex < text.Length)
			{
				string atom;
				var bestAtomMatch = (from atomSpecification in (isPreviousSeparator.GetValueOrDefault() ? (IEnumerable<AtomSpecification>)_atomSpecifications : _separatorSpecifications)
									 let match = new
									 {
										 Success = atomSpecification.TryIdentifty(text, out atom, currentIndex),
										 Value = atom,
										 Specification = atomSpecification
									 }
									 where match.Success
									 orderby match.Value.Length descending
									 select match).FirstOrDefault();
				if (bestAtomMatch == null)
				{
					isPreviousSeparator = null;
					unknownSymbolBuilder.Append(text[currentIndex++]);
				}
				else
				{
					if (unknownSymbolBuilder.Length > 0)
					{
						unknownSymbolExceptons.Add(new ArgumentException(string.Format("Unknown symbol: {0}, line: {1}, column: {2}", unknownSymbolBuilder.ToString(), line, column), "text"));
						unknownSymbolBuilder.Clear();
					}
					scannedAtoms.Add(new ScannedAtom(bestAtomMatch.Specification.Code, line, column, bestAtomMatch.Value));
					isPreviousSeparator = bestAtomMatch.Specification.IsSeparator;
					currentIndex += bestAtomMatch.Value.Length;

					int lastNewLine = bestAtomMatch.Value.LastIndexOf(Environment.NewLine);
					if (lastNewLine != -1)
					{
						column = (uint)(bestAtomMatch.Value.Length - lastNewLine - Environment.NewLine.Length);
						do
						{
							line++;
							lastNewLine = bestAtomMatch.Value.LastIndexOf(Environment.NewLine, 0, lastNewLine);
						} while (lastNewLine != -1);
					}
				}
			}

			if (unknownSymbolExceptons.Count > 0)
				throw new AggregateException("Unknown symbols", unknownSymbolExceptons);
			return scannedAtoms;
		}
		public IList<AtomSpecification> AtomSepcifications
		{
			get
			{
				return _atomSpecifications;
			}
		}
		#endregion

		private readonly IList<AtomSpecification> _atomSpecifications = new List<AtomSpecification>();
	}
}