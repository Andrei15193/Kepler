using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Andrei15193.Kepler.Language.Lexic.Scanners
{
	public sealed class DefaultScanner<TAtomCode>
		: IScanner<TAtomCode>
		where TAtomCode : struct
	{
		#region IScanner Members
		public IReadOnlyList<ScannedAtom<TAtomCode>> Scan(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			int currentIndex = 0;
			int line = 1, column = 1;
			bool? isPreviousSeparator = null;
			StringBuilder unknownSymbolBuilder = new StringBuilder();
			List<ScannedAtom<TAtomCode>> scannedAtoms = new List<ScannedAtom<TAtomCode>>();
			IList<ArgumentException> unknownSymbolExceptons = new List<ArgumentException>();
			IReadOnlyCollection<AtomSpecification<TAtomCode>> _separatorSpecifications = _atomSpecifications.Where(atomSpecification => atomSpecification.IsSeparator)
																											.ToList();
			while (currentIndex < text.Length)
			{
				string atom;
				var bestAtomMatch = (from atomSpecification in (!isPreviousSeparator.HasValue || isPreviousSeparator.Value == true ? (IEnumerable<AtomSpecification<TAtomCode>>)_atomSpecifications : _separatorSpecifications)
									 let match = new
									 {
										 Success = atomSpecification.TryIdentifty(text, out atom, currentIndex),
										 Value = atom,
										 Specification = atomSpecification
									 }
									 where match.Success && match.Value.Length > 0
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
						string unknownSymbol = unknownSymbolBuilder.ToString();
						unknownSymbolExceptons.Add(new ArgumentException(string.Format("Unknown symbol: {0}, line: {1}, column: {2}", unknownSymbol, line, column), "text"));
						column += unknownSymbol.Length;
						unknownSymbolBuilder.Clear();
					}
					if (!bestAtomMatch.Specification.Ignore)
						scannedAtoms.Add(new ScannedAtom<TAtomCode>(bestAtomMatch.Specification.Code, line, column, bestAtomMatch.Value));
					isPreviousSeparator = bestAtomMatch.Specification.IsSeparator;
					currentIndex += bestAtomMatch.Value.Length;

					int newLineIndex = bestAtomMatch.Value.IndexOf(Environment.NewLine);
					if (newLineIndex == -1)
						column += bestAtomMatch.Value.Length;
					else
						do
						{
							line++;
							column = (bestAtomMatch.Value.Length - newLineIndex - Environment.NewLine.Length + 1);
							newLineIndex = bestAtomMatch.Value.IndexOf(Environment.NewLine, newLineIndex + Environment.NewLine.Length);
						} while (newLineIndex != -1);
				}
			}

			if (unknownSymbolExceptons.Count > 0)
				throw new AggregateException("Unknown symbols", unknownSymbolExceptons);
			return scannedAtoms;
		}
		public IList<AtomSpecification<TAtomCode>> AtomSepcifications
		{
			get
			{
				return _atomSpecifications;
			}
		}
		#endregion

		private readonly IList<AtomSpecification<TAtomCode>> _atomSpecifications = new List<AtomSpecification<TAtomCode>>();
	}
}