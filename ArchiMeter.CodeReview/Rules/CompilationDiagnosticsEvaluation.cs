// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDiagnosticsEvaluation.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CompilationDiagnosticsEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers;
	using Roslyn.Compilers.CSharp;

	internal class CompilationDiagnosticsEvaluation : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.CompilationUnit;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			// Roslyn does not handle async await keywords.
			var diagnostics = node.GetDiagnostics();
			if (diagnostics.Any(d => d.Info.Severity != DiagnosticSeverity.Info))
			{
				var quality = diagnostics.Select(d => (d.Info.Severity == DiagnosticSeverity.Warning && d.Info.IsWarningAsError) ? DiagnosticSeverity.Error : d.Info.Severity)
										 .Any(s => s == DiagnosticSeverity.Error)
								  ? CodeQuality.Broken
								  : CodeQuality.NeedsReview;
				return new EvaluationResult
						   {
							   Comment = "Compilation Diagnostics", // string.Join(Environment.NewLine, new[] { "Compilation Diagnostics" }.Concat(diagnostics.Select(d => d.Info.GetMessage()))),
							   Quality = quality, 
							   QualityAttribute = QualityAttribute.CodeQuality, 
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}