using System.Collections.Generic;
namespace Andrei15193.Kepler.Language.Lexic.Scanner
{
	public interface IScanner
	{
		IReadOnlyList<ScannedAtom> Scan(string text);
		IList<AtomSpecification> AtomSepcifications
		{
			get;
		}
	}
}