// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnippetMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using Roslyn.Compilers.CSharp;

	public class SnippetMetricsCalculator
	{
		public Compilation Calculate(string snippet)
		{
			var tree = SyntaxTree.ParseText(snippet);
			var compilation = Compilation.Create("x", syntaxTrees: new[] { tree });
			return compilation;
		}
	}
}
