using System;
using System.Collections;
using System.Collections.Generic;
namespace Andrei15193.Kepler.Extensions.Delegate
{
	public sealed class DelegateEqualityComparer<T>
		: IEqualityComparer<T>
	{
		public DelegateEqualityComparer(Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
		{
			_equals = equals ?? ((T x, T y) => x.Equals(y));
			_getHashCode = getHashCode ?? ((T obj) => obj.GetHashCode());
		}

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

		private readonly Func<T, T, bool> _equals;
		private readonly Func<T, int> _getHashCode;
	}

	public static class DelegateEqualityComparer
	{
		static public IEqualityComparer<T> Create<T>(Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
		{
			return new DelegateEqualityComparer<T>(equals, getHashCode);
		}
	}
}