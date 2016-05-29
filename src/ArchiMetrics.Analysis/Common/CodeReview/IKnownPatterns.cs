// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IKnownPatterns.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IKnownPatterns type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.CodeReview
{
    using System.Collections.Generic;

    public interface IKnownPatterns : IEnumerable<string>
	{
		bool IsExempt(string word);

		void Add(params string[] patterns);

		void Remove(string pattern);

		void Clear();
	}
}
