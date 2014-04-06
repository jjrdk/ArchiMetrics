namespace ArchiMetrics.Analysis.Validation
{
	using ArchiMetrics.Common.Structure;

	public abstract class ValidationResultBase : IValidationResult
	{
		protected ValidationResultBase(bool passed, IModelNode vertex)
		{
			Passed = passed;
			Vertex = vertex;
		}

		public bool Passed { get; private set; }

		public abstract string Value { get; }

		public IModelNode Vertex { get; set; }
	}
}