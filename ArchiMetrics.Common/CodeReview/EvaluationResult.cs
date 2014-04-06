namespace ArchiMetrics.Common.CodeReview
{
	using System;

	[Serializable]
	public class EvaluationResult
	{
		public string ProjectName { get; set; }

		public string ProjectPath { get; set; }

		public string Namespace { get; set; }

		public string TypeName { get; set; }

		public string TypeKind { get; set; }

		public string FilePath { get; set; }

		public string Title { get; set; }

		public string Suggestion { get; set; }

		public string Snippet { get; set; }

		public int LinesOfCodeAffected { get; set; }

		public int ErrorCount { get; set; }

		public CodeQuality Quality { get; set; }

		public QualityAttribute QualityAttribute { get; set; }

		public ImpactLevel ImpactLevel { get; set; }
	}
}
