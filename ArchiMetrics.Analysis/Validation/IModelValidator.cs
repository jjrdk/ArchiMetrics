namespace ArchiMetrics.Analysis.Validation
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Structure;

	public interface IModelValidator
	{
		Task<IEnumerable<IValidationResult>> Validate(string solutionPath, IEnumerable<IModelRule> modelVertices, IEnumerable<TransformRule> transformRules, CancellationToken cancellationToken);
	}
}