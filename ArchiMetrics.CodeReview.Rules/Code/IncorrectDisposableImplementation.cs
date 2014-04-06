namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class IncorrectDisposableImplementation : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.ClassDeclaration; }
		}

		public override string Title
		{
			get { return "Incorrect Dispose pattern implementation"; }
		}

		public override string Suggestion
		{
			get { return "Implement dispose pattern with finalizer and separate disposal of managed and unmanaged resources."; }
		}

		public override CodeQuality Quality
		{
			get { return CodeQuality.NeedsReview; }
		}

		public override QualityAttribute QualityAttribute
		{
			get { return QualityAttribute.CodeQuality | QualityAttribute.Conformance; }
		}

		public override ImpactLevel ImpactLevel
		{
			get { return ImpactLevel.Type; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			if (classDeclaration.BaseList != null && classDeclaration.BaseList.Types.OfType<IdentifierNameSyntax>().Any(t => t.Identifier.ValueText.Contains("IDisposable"))) // t.IsEquivalentTo(SyntaxFactory.ParseTypeName("IDisposable"))))
			{
				var methods = classDeclaration.ChildNodes().OfType<MethodDeclarationSyntax>()
					.Where(m => m.Identifier.ValueText == "Dispose")
					.Where(m =>
						{
							var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
							return m.ParameterList.Parameters.Count == 0
										   || (m.ParameterList.Parameters.Count == 1 && m.ParameterList.Parameters[0].Type.EquivalentTo(predefinedType));
						}).ToArray();
				var destructor = classDeclaration
					.ChildNodes()
					.OfType<DestructorDeclarationSyntax>()
					.FirstOrDefault(d => d.Body.ChildNodes().Any(InvokesDispose));
				if (methods.Length < 2 || destructor == null)
				{
					return new EvaluationResult
					{
						Snippet = node.ToFullString()
					};
				}
			}

			return null;
		}

		private bool InvokesDispose(SyntaxNode node)
		{
			var expression = node as ExpressionStatementSyntax;
			if (expression != null)
			{
				var invocation = expression.Expression as InvocationExpressionSyntax;
				if (invocation != null)
				{
					var identifier = invocation.Expression as IdentifierNameSyntax;
					if (identifier != null
						&& identifier.Identifier.ValueText == "Dispose"
						&& invocation.ArgumentList != null
						&& invocation.ArgumentList.Arguments.Count == 1
						&& invocation.ArgumentList.Arguments[0].EquivalentTo(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression, SyntaxFactory.Token(SyntaxKind.FalseKeyword)))))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}