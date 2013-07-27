// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownWordList.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the KnownWordList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI
{
	using System.Collections.Generic;
	using Common;

	public class KnownWordList : IKnownWordList
	{
		private static readonly HashSet<string> KnownStrings = new HashSet<string> { };

		public bool IsExempt(string word)
		{
			return KnownStrings.Contains(word);
		}
	}
}
