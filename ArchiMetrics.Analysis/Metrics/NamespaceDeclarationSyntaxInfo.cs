// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceDeclarationSyntaxInfo.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceDeclarationSyntaxInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using Roslyn.Compilers.Common;

	public sealed class NamespaceDeclarationSyntaxInfo
	{
		public string CodeFile { get; set; }

		public string Name { get; set; }

		public CommonSyntaxNode Syntax { get; set; }
	}
}