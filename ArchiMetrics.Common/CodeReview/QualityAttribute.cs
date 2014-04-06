namespace ArchiMetrics.Common.CodeReview
{
	using System;

	[Flags]
	public enum QualityAttribute
	{
		CodeQuality = 1, 
		Maintainability = 2, 
		Testability = 4, 
		Modifiability = 8, 
		Reusability = 16, 
		Conformance = 32, 
		Security = 64
	}
}
