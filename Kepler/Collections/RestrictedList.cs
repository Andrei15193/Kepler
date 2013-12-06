using System;
using System.Collections;
using System.Collections.Generic;

namespace Andrei15193.Kepler.Collections
{
    public class RestrictedList<T>
        : IList<T>
    {
        public RestrictedList(Func<T, bool> condition, IEnumerable<T> collection = null)
        {
            if (condition != null)
            {
                _condition = condition;
                if (collection != null)
                    foreach (T element in collection)
                        if (_condition(element))
                            _items.Add(element);
                        else
                            throw new ArgumentException("collection contains invalid element: " + element, "collection");
            }
            else
                throw new ArgumentNullException("condition");
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (_condition(item))
                _items.Insert(index, item);
            else
                throw new ArgumentException("item " + item + " is not allowed in collection!", "item");

        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                if (_condition(value))
                    _items[index] = value;
                else
                    throw new ArgumentException("item " + value + " is not allowed in collection!");

            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            if (_condition(item))
                _items.Add(item);
            else
                throw new ArgumentException("item " + item + " is not allowed in collection!", "item");
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return (_condition(item) && _items.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return (_condition(item) && _items.Remove(item));
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private readonly Func<T, bool> _condition;
        private readonly IList<T> _items = new List<T>();
    }
}