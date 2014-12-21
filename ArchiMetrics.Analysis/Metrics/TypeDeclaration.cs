// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDeclaration.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDeclaration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using ArchiMetrics.Common.Metrics;

	internal sealed class TypeDeclaration
	{
		public string Name { get; set; }

		public IEnumerable<TypeDeclarationSyntaxInfo> SyntaxNodes { get; set; }
	}
}
