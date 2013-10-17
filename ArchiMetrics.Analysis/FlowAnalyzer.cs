namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class FlowAnalyzer
	{
		public Task<IEnumerable<IControlFlowAnalysis>> GetControlFlows(IDocument document)
		{
			return GetFlow(document, GetControlFlow);
		}

		public Task<IEnumerable<IDataFlowAnalysis>> GetDataFlows(IDocument document)
		{
			return GetFlow(document, GetDataFlow);
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