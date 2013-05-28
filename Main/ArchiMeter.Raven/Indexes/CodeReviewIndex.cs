// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeReviewIndex.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeReviewIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Abstractions.Indexing;
	using global::Raven.Client.Indexes;

	public class CodeReviewIndex : AbstractIndexCreationTask<ErrorData, ErrorData>
	{
		public CodeReviewIndex()
		{
			Map = errorDatas => from errorData in errorDatas
								select new
								{
									Id = errorData.ProjectName + errorData.ProjectVersion,
									ProjectName = errorData.ProjectName,
									ProjectVersion = errorData.ProjectVersion,
									DistinctLoc = errorData.DistinctLoc,
									Effort = errorData.Effort,
									Error = errorData.Error,
									Occurrences = errorData.Occurrences
								};

			Reduce = data => from ed in data
							 group ed by new { ed.Id, ed.Error } into g
							 select new
							 {
								 Id = g.First().Id,
								 ProjectName = g.First().ProjectName,
								 ProjectVersion = g.First().ProjectVersion,
								 DistinctLoc = g.Sum(x => x.DistinctLoc),
								 Effort = g.Sum(x => x.Effort),
								 Error = g.Key.Error,
								 Occurrences = g.Sum(x => x.Occurrences)
							 };

			Store(e => e.ProjectName, FieldStorage.Yes);
			Store(e => e.ProjectVersion, FieldStorage.Yes);
		}
	}
}