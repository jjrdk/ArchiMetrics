// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnippetMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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
