// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDeclaration.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDeclaration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using System.Collections.Generic;
	using Common.Metrics;

	public sealed class TypeDeclaration
	{
		// Properties
		public string Name { get; set; }

		public IEnumerable<TypeDeclarationSyntaxInfo> SyntaxNodes { get; set; }
	}
}