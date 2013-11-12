// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultComparer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ResultComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using ArchiMetrics.Common;
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
