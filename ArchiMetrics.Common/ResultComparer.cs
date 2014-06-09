// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultComparer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
