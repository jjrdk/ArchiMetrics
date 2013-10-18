namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class SemanticAnalyzer
	{
		private readonly ISemanticModel _model;

		public SemanticAnalyzer(ISemanticModel model)
		{
			_model = model;
		}

		public IEnumerable<ParameterSyntax> GetUnusedParameters(MethodDeclarationSyntax method)
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
				.Where(method =>
				{
					var bodyNodes = method.Body.ChildNodes();
					var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());
					var hasThisReference = dataflow.DataFlowsIn.Any(x => x.Kind == CommonSymbolKind.Parameter && x.Name == "this");
					return !hasThisReference;
				})
				.ToArray();
		}

		private static IDataFlowAnalysis GetDataFlow(
			CommonSyntaxNode start,
			CommonSyntaxNode end,
			ISemanticModel semanticModel)
		{
			var flow = semanticModel.AnalyzeDataFlow(start, end);
			return flow.Succeeded
				? flow
				: null;
		}

		private static IControlFlowAnalysis GetControlFlow(CommonSyntaxNode start, CommonSyntaxNode end, ISemanticModel semanticModel)
		{
			var flow = semanticModel.AnalyzeControlFlow(start, end);
			return flow.Succeeded
				? flow
				: null;
		}

		private static async Task<IEnumerable<T>> GetFlow<T>(
			IDocument document,
			Func<CommonSyntaxNode, CommonSyntaxNode, ISemanticModel, T> flowLoader)
		{
			var modelTask = document.GetSemanticModelAsync();
			var methodsTask = document.GetSyntaxRootAsync();

			await Task.WhenAll(modelTask, methodsTask);

			var methods = methodsTask.Result
				.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Select(x => x.Body)
				.Where(x => x != null)
				.Select(x => x.ChildNodes())
				.Where(x => x.Any())
				.Select(x => flowLoader(x.First(), x.Last(), modelTask.Result))
				.Where(x => x != null)
				.ToArray();

			return methods;
		}
	}
}