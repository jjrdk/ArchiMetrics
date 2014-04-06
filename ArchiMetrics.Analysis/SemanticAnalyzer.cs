namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public class SemanticAnalyzer
	{
		private readonly SemanticModel _model;

		public SemanticAnalyzer(SemanticModel model)
		{
			_model = model;
		}

		public IEnumerable<ParameterSyntax> GetUnusedParameters(BaseMethodDeclarationSyntax method)
		{
			if (method.ParameterList.Parameters.Count == 0 || method.Body == null || !method.Body.ChildNodes().Any())
			{
				return new ParameterSyntax[0];
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());

			var usedParameterNames = dataflow.DataFlowsIn
				.Where(x => x.Kind == SymbolKind.Parameter)
				.Select(x => x.Name)
				.ToArray();

			var unusedParameters = method.ParameterList.Parameters
				.Where(p => !usedParameterNames.Contains(p.Identifier.ValueText))
				.ToArray();
			return unusedParameters;
		}

		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "TypeDeclaration constraint intended.")]
		public IEnumerable<MethodDeclarationSyntax> GetPossibleStaticMethods(TypeDeclarationSyntax type)
		{
			return type.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Where(x => !x.Modifiers.Any(SyntaxKind.StaticKeyword))
				.Where(CanBeMadeStatic)
				.ToArray();
		}

		public bool CanBeMadeStatic(BaseMethodDeclarationSyntax method)
		{
			if (method.Modifiers.Any(SyntaxKind.StaticKeyword)
				|| method.Body == null
				|| !method.Body.ChildNodes().Any())
			{
				return false;
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());
			var hasThisReference = dataflow.DataFlowsIn
				.Any(x => x.Kind == SymbolKind.Parameter && x.Name == SyntaxFactory.Token(SyntaxKind.ThisKeyword).ToFullString());
			return !hasThisReference;
		}
	}
}