namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	public interface IMemberMetric : ICodeMetric
	{
		MemberMetricKind Kind { get; }

		string CodeFile { get; }

		int LineNumber { get; }

		IEnumerable<ITypeCoupling> ClassCouplings { get; }

		int NumberOfParameters { get; }

		int NumberOfLocalVariables { get; }

		int AfferentCoupling { get; }

		double GetVolume();

		IHalsteadMetrics GetHalsteadMetrics();
	}
}