// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDeclarationSyntaxInfo.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDeclarationSyntaxInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal class TypeDeclarationSyntaxInfo
	{
		public TypeDeclarationSyntaxInfo(string codeFile, string name, TypeDeclarationSyntax syntax)
		{
			CodeFile = codeFile;
			Name = name;
			Syntax = syntax;
		}

		public string CodeFile { get; }

		public string Name { get; }

		public TypeDeclarationSyntax Syntax { get; set; }
	}
}
