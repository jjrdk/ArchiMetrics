namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface INamespaceMetric : ICodeMetric
	{
		IEnumerable<TypeCoupling> ClassCouplings { get; }
		
		int DepthOfInheritance { get; }
		
		IEnumerable<ITypeMetric> TypeMetrics { get; }
	}
}