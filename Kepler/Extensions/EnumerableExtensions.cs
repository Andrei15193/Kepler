using System;
using System.Linq;
using System.Collections.Generic;
namespace Andrei15193.Kepler.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Append<T>(this IEnumerable<T> items, T appendant)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			foreach (T item in items)
				yield return item;
			yield return appendant;
		}
		public static Queue<T> ToQueue<T>(this IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			return new Queue<T>(items);
		}
		public static Stack<T> ToStack<T>(this IEnumerable<T> items)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			return new Stack<T>(items);
		}
		public static int IndexOf<T>(this IEnumerable<T> items, T item, IEqualityComparer<T> equalityComaprer = null)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			if (equalityComaprer == null)
				return _IndexOf(items, item);
			return _IndexOf(items, item, (first, second) => equalityComaprer.Equals(first, second));
		}

		private static int _IndexOf<T>(IEnumerable<T> items, T item, Func<T, T, bool> equalityComparer = null)
		{
			if (items == null)
				throw new ArgumentNullException("items");

			if (equalityComparer == null)
				if (item == null)
					equalityComparer = ((first, second) => (second == null));
				else
					equalityComparer = ((first, second) => first.Equals(second));

			bool found = false;
			int index = -1;

			using (IEnumerator<T> itemEnumerator = items.GetEnumerator())
				while (itemEnumerator.MoveNext() && !found)
				{
					index++;
					found = equalityComparer(item, itemEnumerator.Current);
				}

			if (found)
				return index;
			return -1;
		}
	}
}