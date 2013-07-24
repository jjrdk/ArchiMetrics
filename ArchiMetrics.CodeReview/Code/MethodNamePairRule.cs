namespace ArchiMetrics.CodeReview.Code
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal abstract class MethodNamePairRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		protected abstract string BeginToken { get; }

		protected abstract string PairToken { get; }

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var method = (MethodDeclarationSyntax)node;
			if (!HasMatchingMethod(BeginToken, PairToken, method))
			{
				return new EvaluationResult
						   {
							   Comment = "Pair method missing",
							   ErrorCount = 1,
							   ImpactLevel = ImpactLevel.Type,
							   Quality = CodeQuality.NeedsRefactoring,
							   QualityAttribute = QualityAttribute.Conformance,
							   Snippet = method.ToFullString()
						   };
			}

			return null;
		}

		private bool HasMatchingMethod(string start, string match, MethodDeclarationSyntax method)
		{
			var methodName = method.Identifier.ValueText;
			if (methodName.StartsWith(start, StringComparison.InvariantCultureIgnoreCase))
			{
				var parentClass = FindClassParent(method);
				var pairMethodName = Regex.Replace(methodName, "^" + start, match);
				return parentClass
					.ChildNodes()
					.OfType<MethodDeclarationSyntax>()
					.Any(m => m.Identifier.ValueText == pairMethodName);
			}

			return true;
		}
	}
}
