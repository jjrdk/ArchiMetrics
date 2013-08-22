// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpellChecker.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ISpellChecker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.CodeReview
{
	using System;

	public interface ISpellChecker : IDisposable
	{
		bool Spell(string word);
	}
}
