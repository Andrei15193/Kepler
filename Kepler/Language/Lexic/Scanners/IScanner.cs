using System.Collections.Generic;
namespace Andrei15193.Kepler.Language.Lexic.Scanners
{
	public interface IScanner<TAtomCode>
		where TAtomCode : struct
	{
		IReadOnlyList<ScannedAtom<TAtomCode>> Scan(string text);
		IList<AtomSpecification<TAtomCode>> AtomSepcifications
		{
			get;
		}
	}
}