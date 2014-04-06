namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using ArchiMetrics.Common.CodeReview;

	internal class ResultComparer : IEqualityComparer<EvaluationResult>
	{
		public bool Equals(EvaluationResult x, EvaluationResult y)
		{
			return x == null
				       ? y == null
				       : y != null
				         && (x.ProjectPath == y.ProjectPath
				             && x.FilePath == y.FilePath
				             && x.Quality == y.Quality
				             && x.LinesOfCodeAffected == y.LinesOfCodeAffected
				             && x.Title == y.Title
				             && x.Snippet == y.Snippet);
		}

		public int GetHashCode(EvaluationResult obj)
		{
			return obj.GetHashCode();
		}
	}
}
