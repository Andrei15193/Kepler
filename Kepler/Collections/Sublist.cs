using System;
using System.Collections;
using System.Collections.Generic;

namespace Andrei15193.Kepler.Collections
{
    /// <summary>
    /// Represents a readonly sublist proxy.
    /// </summary>
    /// <typeparam name="T">The type of elements in the read-only list.</typeparam>
    /// <remarks>
    /// This collection is optimized for readonly lists.
    /// </remarks>
    public sealed class Sublist<T>
        : IReadOnlyList<T>
    {
        /// <summary>
        /// Creates a sublist for the given list. Instances are only wrappers meaning that any
        /// changes in the given list may be reflected in the sublist. This collection is
        /// optimized for readonly collections (where such scenarios are not possible).
        /// </summary>
        /// <param name="list">
        /// The list to wrap.
        /// </param>
        /// <param name="startIndex">
        /// The startIndex in the given list where the sublist will start.
        /// </param>
        /// <param name="length">
        /// The length of the sublist.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when list is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when startIndex is out of bounds of the given collection or not equal with
        /// the length of the given collection.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when length is negative.
        /// </exception>
        /// <remarks>
        /// <para>
        /// If the provided length exceeds the sublist length, then the maximum possible number
        /// of elementes is considered to be the length of the sublist. In case the source
        /// collection contains more elements after the sublist creation, the length of the
        /// sublist is not recalculated.
        /// </para>
        /// </remarks>
        public Sublist(IReadOnlyList<T> list, int startIndex = 0, int? length = null)
        {
            if (list != null)
                if (0 <= startIndex
                    && (startIndex < list.Count
                        || list.Count - startIndex == 0))
                    if (!length.HasValue
                        || length.Value >= 0)
                    {
                        Sublist<T> sublist = list as Sublist<T>;

                        if (sublist != null)
                        {
                            _list = sublist._list;
                            _startIndex = sublist._startIndex + startIndex;

                            if (length.HasValue)
                                _length = Math.Min(length.Value, sublist._length);
                            else
                                _length = sublist._length;
                        }
                        else
                        {
                            _list = list;
                            _startIndex = startIndex;

                            if (length.HasValue)
                                _length = Math.Min(length.Value, list.Count - startIndex);
                            else
                                _length = list.Count - startIndex;
                        }
                    }
                    else
                        throw new ArgumentException("The provided length cannot be negative!");
                else
                    throw new ArgumentOutOfRangeException("startIndex");
            else
                throw new ArgumentNullException("list");
        }

        /// <summary>
        /// Creates a sublist for the given list. Instances are only wrappers meaning that any
        /// changes in the given list may be reflected in the sublist. This collection is
        /// optimized for readonly collections (where such scenarios are not possible).
        /// </summary>
        /// <param name="sublist">
        /// The list to wrap.
        /// </param>
        /// <param name="startIndex">
        /// The startIndex in the given list where the sublist will start.
        /// </param>
        /// <param name="length">
        /// The length of the sublist.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when sublist is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when startIndex is out of bounds of the given collection or not equal with
        /// the length of the given collection.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when length is negative.
        /// </exception>
        /// <remarks>
        /// <para>
        /// If the provided length exceeds the sublist length, then the maximum possible number
        /// of elementes is considered to be the length of the sublist. In case the source
        /// collection contains more elements after the sublist creation, the length of the
        /// sublist is not recalculated.
        /// </para>
        /// <para>
        /// The provided sublist is taken as a prototype for the current instance to avoid
        /// overwrapping and unjustified recursion (accessing element via indexer).
        /// </para>
        /// </remarks>
        public Sublist(Sublist<T> sublist, int startIndex = 0, int? length = null)
        {
            if (sublist != null)
                if (0 <= startIndex
                    && (startIndex < sublist.Count
                        || sublist.Count - startIndex == 0))
                    if (!length.HasValue
                        || length.Value >= 0)
                    {
                        _list = sublist._list;
                        _startIndex = sublist._startIndex + startIndex;

                        if (length.HasValue)
                            _length = Math.Min(length.Value, sublist._length);
                        else
                            _length = sublist._length;
                    }
                    else
                        throw new ArgumentException("The provided length cannot be negative!");
                else
                    throw new ArgumentOutOfRangeException("startIndex");
            else
                throw new ArgumentNullException("list");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.IEnumerator<T> that can be used to iterate through the
        /// collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int currentIndex = 0; currentIndex < _length; currentIndex++)
                yield return _list[currentIndex + _startIndex];
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through the
        /// collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// Thrown when the index is out the bounds of the sublist.
        /// </exception>
        public T this[int index]
        {
            get
            {
                if (0 <= index && index < _length)
                    return _list[index + _startIndex];
                else
                    throw new IndexOutOfRangeException("startIndex");
            }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection.
        /// </returns>
        public int Count
        {
            get
            {
                return _length;
            }
        }

        private readonly int _startIndex;
        private readonly int _length;
        private readonly IReadOnlyList<T> _list;
    }
}
