namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface ITypeDocumentation : IDocumentation
	{
		IEnumerable<TypeParameterDocumentation> TypeParameters { get; } 
	}
}