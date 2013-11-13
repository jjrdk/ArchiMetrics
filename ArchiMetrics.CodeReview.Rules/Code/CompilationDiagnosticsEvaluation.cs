// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilationDiagnosticsEvaluation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.Broken;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Project;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			// Roslyn does not handle async await keywords.
			var diagnostics = node.GetDiagnostics();
			if (diagnostics.Any(d => d.Info.Severity != DiagnosticSeverity.Info))
			{
				return new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}
