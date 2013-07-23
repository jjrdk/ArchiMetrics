namespace ArchiMetrics.CodeReview.Semantic
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;
	using Rules;

	internal class LackOfCohesionOfMethodsRule : EvaluationBase, ISemanticEvaluation
	{
		private static readonly CommonSymbolKind[] MemberKinds =
		{
			CommonSymbolKind.Event, 
			CommonSymbolKind.Method, 
			CommonSymbolKind.Property
		};

		private int _threshold = 10;

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.ClassDeclaration; }
		}

		public void SetThreshold(int threshold)
		{
			_threshold = threshold;
		}

		public EvaluationResult Evaluate(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			var symbol = (ITypeSymbol)semanticModel.GetDeclaredSymbol(classDeclaration);
			var members = symbol.GetMembers();

			var memberCount = members.Where(x => MemberKinds.Contains(x.Kind)).Count();
			if (memberCount < _threshold)
			{
				return null;
			}

			var fields = members.Where(x => x.Kind == CommonSymbolKind.Field).ToArray();
			var fieldCount = fields.Length;

			if (fieldCount < _threshold)
			{
				return null;
			}

			var sumFieldUsage = (double)fields.Sum(f => f.FindReferences(solution)
				.SelectMany(x => x.Locations)
				.Count());

			var lcomhs = (memberCount - sumFieldUsage / fieldCount) / (memberCount - 1);
			if (lcomhs < 1)
			{
				return null;
			}

			var snippet = node.ToFullString();
			return new EvaluationResult
				   {
					   Comment = "High lack of cohesion detected.",
					   ImpactLevel = ImpactLevel.Type,
					   LinesOfCodeAffected = GetLinesOfCode(snippet),
					   Quality = CodeQuality.NeedsRefactoring,
					   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Maintainability,
					   Snippet = snippet
				   };
		}
	}
}