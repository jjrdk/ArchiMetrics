namespace ArchiMeter.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal abstract class TriviaEvaluationBase : ITriviaEvaluation
	{
		public abstract SyntaxKind EvaluatedKind { get; }

		public EvaluationResult Evaluate(SyntaxTrivia node)
		{
			var result = EvaluateImpl(node);
			if (result != null)
			{
				var sourceTree = node.GetLocation().SourceTree;
				var filePath = sourceTree.FilePath;
				var unitNamespace = GetCompilationUnitNamespace(sourceTree.GetRoot());
				result.Namespace = unitNamespace;
				result.FilePath = filePath;
				result.LinesOfCodeAffected = 0;
			}

			return result;
		}

		protected abstract EvaluationResult EvaluateImpl(SyntaxTrivia node);

		private static string GetCompilationUnitNamespace(CompilationUnitSyntax node)
		{
			var namespaceDeclaration = node.DescendantNodes()
				.FirstOrDefault(n => n.Kind == SyntaxKind.NamespaceDeclaration);

			return namespaceDeclaration == null ? string.Empty : ((NamespaceDeclarationSyntax)namespaceDeclaration).Name.GetText().ToString().Trim();
		}
	}
}