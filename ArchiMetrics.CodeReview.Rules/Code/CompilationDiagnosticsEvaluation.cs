// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDiagnosticsEvaluation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CompilationDiagnosticsEvaluation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
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

		public override string Title
		{
			get
			{
				return "Compilation Failure";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Check the compilation error for details about reason for failure.";
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
							   Quality = quality, 
							   QualityAttribute = QualityAttribute.CodeQuality, 
							   ImpactLevel = ImpactLevel.Project,
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}
