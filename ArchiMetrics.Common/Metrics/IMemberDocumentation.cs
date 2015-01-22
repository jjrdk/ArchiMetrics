namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface IMemberDocumentation : IDocumentation
	{
		IEnumerable<ParameterDocumentation> Parameters { get; }

		IEnumerable<TypeParameterDocumentation> TypeParameters { get; }

		IEnumerable<ExceptionDocumentation> Exceptions { get; }
	}
}