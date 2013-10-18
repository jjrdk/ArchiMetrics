namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	public class SemanticAnalyzer
	{
		private readonly ISemanticModel _model;

		public SemanticAnalyzer(ISemanticModel model)
		{
			_model = model;
		}

		public IEnumerable<ParameterSyntax> GetUnusedParameters(BaseMethodDeclarationSyntax method)
		{
			if (method.ParameterList.Parameters.Count == 0)
			{
				return new ParameterSyntax[0];
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());

			var usedParameterNames = dataflow.DataFlowsIn
				.Where(x => x.Kind == CommonSymbolKind.Parameter)
				.Select(x => x.Name)
				.ToArray();

			var unusedParameters = method.ParameterList.Parameters
				.Where(p => !usedParameterNames.Contains(p.Identifier.ValueText))
				.ToArray();
			return unusedParameters;
		}

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
			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());
			var hasThisReference = dataflow.DataFlowsIn
				.Any(x => x.Kind == CommonSymbolKind.Parameter && x.Name == Syntax.Token(SyntaxKind.ThisKeyword).ToFullString());
			return !hasThisReference;
		}
	}
}