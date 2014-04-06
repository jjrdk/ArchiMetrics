namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class MultipleReturnStatementsErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MethodDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Multiple Return Statements";
			}
		}
		
		public override string Suggestion
		{
			get
			{
				return "If your company's coding standards requires only a single exit point, then refactor method to have only single return statement.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsReview;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Conformance;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Member;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			var returnStatements = methodDeclaration.DescendantNodes().Where(n => n.IsKind(SyntaxKind.ReturnStatement)).ToArray();
			if (returnStatements.Length > 1)
			{
				return new EvaluationResult
						   { 
							   Snippet = node.ToFullString(), 
							   ErrorCount = returnStatements.Length
						   };
			}

			return null;
		}
	}
}
