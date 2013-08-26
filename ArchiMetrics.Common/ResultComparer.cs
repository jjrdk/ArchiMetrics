// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultComparer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ResultComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
							 && x.LinesOfCodeAffected == y.LinesOfCodeAffected
							 && x.Title == y.Title
							 && x.Suggestion == y.Suggestion
							 && x.Snippet == y.Snippet);
		}

		public int GetHashCode(EvaluationResult obj)
		{
			return obj.GetHashCode();
		}
	}
}
