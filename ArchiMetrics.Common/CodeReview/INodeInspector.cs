namespace ArchiMetrics.Common.CodeReview
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;

	public interface INodeInspector
	{
		Task<IEnumerable<EvaluationResult>> Inspect(Solution solution);
		
		Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, string projectName, SyntaxNode node, SemanticModel semanticModel, Solution solution);
	}
}
