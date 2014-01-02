using System.Collections.Generic;
using Andrei15193.Kepler.Collections;
namespace Andrei15193.Kepler.Extensions
{
    /// <summary>
    /// A class containing extension methods for the
    /// <see cref="System.Collections.Generic.IReadOnlyList&lt;T&gt;"/> interface.
    /// </summary>
    public static class IReadOnlyListExtensions
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
        static public IReadOnlyList<T> Sublist<T>(this IReadOnlyList<T> list, int startIndex = 0, int? length = null)
        {
            return new Sublist<T>(list, startIndex, length);
        }
    }
}