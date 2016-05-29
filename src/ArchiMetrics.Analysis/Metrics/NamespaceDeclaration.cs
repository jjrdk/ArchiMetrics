// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceDeclaration.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceDeclaration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;

	public sealed class NamespaceDeclaration
	{
		public string Name { get; set; }

		public IEnumerable<NamespaceDeclarationSyntaxInfo> SyntaxNodes { get; set; }
	}
}
