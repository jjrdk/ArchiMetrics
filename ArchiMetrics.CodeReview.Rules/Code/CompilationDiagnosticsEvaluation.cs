namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

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
			if (diagnostics.Any(d => d.Severity != DiagnosticSeverity.Info))
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
