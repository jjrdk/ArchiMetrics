// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownPatterns.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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

	internal class KnownPatterns : INotifyCollectionChanged, IKnownPatterns
	{
		private readonly List<Regex> _regexes = new List<Regex>();

		public KnownPatterns()
		{
			((IKnownPatterns)this).Add("Microsoft", @"^\d\.\d\.\d{1,5}\.\d$", @"Runtime");
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

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

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<string> GetEnumerator()
		{
			return _regexes.Select(x => x.ToString()).GetEnumerator();
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
			var toRemove = _regexes.Where(x => x.ToString() == pattern).AsArray();
			foreach (var valueHolder in toRemove)
			{
				Remove(valueHolder);
			}
		}

		void IKnownPatterns.Clear()
		{
			Clear();
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
