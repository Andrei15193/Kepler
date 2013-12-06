using System;
using System.Collections;
using System.Collections.Generic;

namespace Andrei15193.Kepler.Relay
{
    public sealed class RelayEqualityComparer<T>
        : IEqualityComparer<T>
    {
        public RelayEqualityComparer(Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
        {
            if (equals == null)
                _equals = (T x, T y) => x.Equals(y);
            else
                _equals = equals;
            if (getHashCode == null)
                _getHashCode = (T obj) => obj.GetHashCode();
            else
                _getHashCode = getHashCode;
        }

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }

        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _getHashCode;
    }

    public sealed class RelayEqualityComparer
        : IEqualityComparer
    {
        static public IEqualityComparer<T> Create<T>(Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
        {
            return new RelayEqualityComparer<T>(equals, getHashCode);
        }

        public RelayEqualityComparer(Func<object, object, bool> equals = null, Func<object, int> getHashCode = null)
        {
            if (equals == null)
                _equals = (object x, object y) => x.Equals(y);
            else
                _equals = equals;
            if (getHashCode == null)
                _getHashCode = (object obj) => obj.GetHashCode();
            else
                _getHashCode = getHashCode;
        }

        public new bool Equals(object x, object y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return _getHashCode(obj);
        }

        private readonly Func<object, object, bool> _equals;
        private readonly Func<object, int> _getHashCode;
    }

}
