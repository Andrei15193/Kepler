using System;
using System.Collections;
using System.Collections.Generic;

namespace Andrei15193.Kepler.Relay
{
    public sealed class RelayComparer<T>
        : IComparer<T>, IEqualityComparer<T>
    {
        public RelayComparer(Func<T, T, int> comparer, Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
        {
            if (comparer != null)
            {
                _comparer = comparer;
                _equals = (equals ?? ((T x, T y) => object.Equals(x, y)));
                _getHashCode = (getHashCode ?? (obj => obj.GetHashCode()));
            }
            else
                throw new ArgumentNullException("comparer");
        }

        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (obj != null)
                return _getHashCode(obj);
            else
                throw new ArgumentNullException("obj");
        }

        private readonly Func<T, T, int> _comparer;
        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _getHashCode;
    }

    public sealed class RelayComparer
        : IComparer, IEqualityComparer
    {
        static public RelayComparer<T> Create<T>(Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
            where T : IComparable<T>
        {
            return new RelayComparer<T>(((x, y) =>
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

        static public RelayComparer<T> Create<T>(Func<T, T, int> comparer, Func<T, T, bool> equals = null, Func<T, int> getHashCode = null)
        {
            return new RelayComparer<T>(comparer, equals, getHashCode);
        }

        public RelayComparer(Func<object, object, int> comparer, Func<object, object, bool> equals = null, Func<object, int> getHashCode = null)
        {
            if (comparer != null)
            {
                _comparer = comparer;
                _equals = (equals ?? object.Equals);
                _getHashCode = (getHashCode ?? (obj => obj.GetHashCode()));
            }
            else
                throw new ArgumentNullException("comparer");
        }

        public int Compare(object x, object y)
        {
            return _comparer(x, y);
        }

        new public bool Equals(object x, object y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(object obj)
        {
            if (obj != null)
                return _getHashCode(obj);
            else
                throw new ArgumentNullException("obj");
        }
        
        private readonly Func<object, object, int> _comparer;
        private readonly Func<object, object, bool> _equals;
        private readonly Func<object, int> _getHashCode;
    }
}
