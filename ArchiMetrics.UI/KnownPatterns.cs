// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownPatterns.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the KnownPatterns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;

	internal class KnownPatterns : ICollection<Regex>, INotifyCollectionChanged, IKnownPatterns
	{
		private readonly List<Regex> _regexes = new List<Regex>();

		public KnownPatterns()
		{
			((IKnownPatterns)this).Add("Microsoft", @"^\d\.\d\.\d{1,5}\.\d$", @"Runtime");
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		public int Count
		{
			get { return _regexes.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
		/// </returns>
		public bool IsReadOnly
		{
			get { return false; }
		}

		bool IKnownPatterns.IsExempt(string word)
		{
			return _regexes.Any(r => r.IsMatch(word));
		}

		void IKnownPatterns.Add(params string[] patterns)
		{
			var items = patterns.WhereNotNullOrWhitespace().Select(x => new Regex(x, RegexOptions.Compiled));
			AddMany(items);
		}

		void IKnownPatterns.Remove(string pattern)
		{
			var toRemove = _regexes.Where(x => x.ToString() == pattern).ToArray();
			foreach (var valueHolder in toRemove)
			{
				Remove(valueHolder);
			}
		}

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public void Add(Regex item)
		{
			if (item == null)
			{
				return;
			}

			AddMany(new[] { item });
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
		public void Clear()
		{
			_regexes.Clear();
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			OnCollectionChanged(args);
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <returns>
		/// <code>true</code> if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		public bool Contains(Regex item)
		{
			return _regexes.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
		public void CopyTo(Regex[] array, int arrayIndex)
		{
			_regexes.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <returns>
		/// <code>true</code> if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public bool Remove(Regex item)
		{
			var index = _regexes.IndexOf(item);
			var result = _regexes.Remove(item);
			if (result)
			{
				var args = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Remove,
					item,
					index);
				OnCollectionChanged(args);
			}

			return result;
		}

		void IKnownPatterns.Clear()
		{
			Clear();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Regex> GetEnumerator()
		{
			return _regexes.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void AddMany(IEnumerable<Regex> items)
		{
			var list = items.ToList();
			foreach (var item in list)
			{
				_regexes.Add(item);
			}

			var args = new NotifyCollectionChangedEventArgs(
				NotifyCollectionChangedAction.Add,
				list);
			OnCollectionChanged(args);
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			var handler = CollectionChanged;
			if (handler != null)
			{
				handler(this, args);
			}
		}
	}
}
