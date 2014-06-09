// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnippetMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SnippetMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	public class SnippetMetricsCalculator
	{
		public Compilation Calculate(string snippet)
		{
			var tree = CSharpSyntaxTree.ParseText(snippet);
			var compilation = CSharpCompilation.Create("x", syntaxTrees: new[] { tree });
			return compilation;
		}
	}
}
