namespace ArchiMetrics.Analysis.Validation
{
	using ArchiMetrics.Common.Structure;

	public interface IValidationResult
	{
		bool Passed { get; }

		string Value { get; }

		IModelNode Vertex { get; }
	}
}