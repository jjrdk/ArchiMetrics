namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface IMemberMetric : ICodeMetric
	{
		MemberMetricKind Kind { get; }
		
		string CodeFile { get; }
		
		int LineNumber { get; }
		
		int LogicalComplexity { get; }
		
		IEnumerable<TypeCoupling> ClassCouplings { get; }
		
		int NumberOfParameters { get; }
		
		int NumberOfLocalVariables { get; }

		double GetVolume();
	}
}