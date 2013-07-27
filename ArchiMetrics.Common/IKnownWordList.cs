// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKnownWordList.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IKnownWordList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	public interface IKnownWordList
	{
		bool IsExempt(string word);
	}
}
