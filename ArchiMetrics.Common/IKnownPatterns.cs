// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKnownPatterns.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IKnownPatterns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	public interface IKnownPatterns
	{
		bool IsExempt(string word);

		void Add(params string[] patterns);

		void Clear();
	}
}
