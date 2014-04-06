namespace ArchiMetrics.Common
{
	using System.Collections.Generic;
	using ArchiMetrics.Common.CodeReview;

	public class ResultComparer : IEqualityComparer<EvaluationResult>
	{
		public bool Equals(EvaluationResult x, EvaluationResult y)
		{
			return x == null
					   ? y == null
					   : y != null
						 && (x.ProjectPath == y.ProjectPath
							 && x.FilePath == y.FilePath
							 && x.Quality == y.Quality
							 && x.QualityAttribute == y.QualityAttribute
							 && x.LinesOfCodeAffected == y.LinesOfCodeAffected
							 && x.Title == y.Title
							 && x.Suggestion == y.Suggestion
							 && x.Namespace == y.Namespace
							 && x.Snippet == y.Snippet
							 && x.TypeKind == y.TypeKind
							 && x.TypeName == y.TypeName
							 && x.ImpactLevel == y.ImpactLevel
							 && x.ErrorCount == y.ErrorCount);
		}

		public int GetHashCode(EvaluationResult obj)
		{
			return obj.GetHashCode();
		}
	}
}
