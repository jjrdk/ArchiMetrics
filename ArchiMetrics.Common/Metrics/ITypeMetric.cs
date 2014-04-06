namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface ITypeMetric : ICodeMetric
	{
		TypeMetricKind Kind { get; }
		
		IEnumerable<IMemberMetric> MemberMetrics { get; }

		int DepthOfInheritance { get; }
		
		IEnumerable<ITypeCoupling> ClassCouplings { get; }
		
		int ClassCoupling { get; }
	}
}