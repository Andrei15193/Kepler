using System;
using System.Collections.Generic;
namespace Andrei15193.Kepler.Extensions.Delegate
{
	public sealed class DelegateComparer<T>
		: IComparer<T>, IEqualityComparer<T>
	{
		public DelegateComparer(Func<T, T, int> comparer, Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
		{
			if (comparer == null)
				throw new ArgumentNullException("comparer");
			_comparer = comparer;
			_equals = (equals ?? ((T x, T y) => (_comparer(x, y) == 0)));
			_getHashCode = (getHashCode ?? (obj => obj.GetHashCode()));
		}

		#region IComparer<T> Members
		public int Compare(T x, T y)
		{
			return _comparer(x, y);
		}
		#endregion
		#region IEqualityComparer<T> Members
		public bool Equals(T x, T y)
		{
			return _equals(x, y);
		}
		public int GetHashCode(T obj)
		{
			return _getHashCode(obj);
		}
		#endregion

		private readonly Func<T, T, int> _comparer;
		private readonly Func<T, T, bool> _equals;
		private readonly Func<T, int> _getHashCode;
	}

	public static class DelegateComparer
	{
		static public DelegateComparer<T> Create<T>(Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
			where T : IComparable<T>
		{
			return new DelegateComparer<T>(((x, y) =>
				{
					if (x == null)
						if (y == null)
							return 0;
						else
							return -y.CompareTo(x);
					else
						return x.CompareTo(y);
				}), equals, getHashCode);
		}
		static public DelegateComparer<T> Create<T>(Func<T, T, int> comparer, Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
		{
			return new DelegateComparer<T>(comparer, equals, getHashCode);
		}
	}
}