namespace ArchiMetrics.Analysis.Validation
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Structure;

	public interface IModelRule
	{
		Task<IEnumerable<IValidationResult>> Validate(IModelNode modelTree);
	}
}