// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCountByProjectVersionIndex.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorCountByProjectVersionIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class ErrorCountByProjectVersionIndex : AbstractIndexCreationTask<EvaluationResultDocument, ErrorCountByProjectVersionIndex.ErrorCountReduction>
	{
		public ErrorCountByProjectVersionIndex()
		{
			Map = docs => from doc in docs
						  select new
									 {
										 ProjectName = doc.ProjectName,
										 ProjectVersion = doc.ProjectVersion,
										 Count = doc.Results.Sum(r => r.ErrorCount)
									 };

			Reduce = redux => from r in redux
							  group r by r.ProjectName + r.ProjectVersion
								  into g
								  select new
											 {
												 ProjectName = g.First().ProjectName,
												 ProjectVersion = g.First().ProjectVersion,
												 Count = g.Sum(x => x.Count)
											 };
		}

		public override string IndexName
		{
			get
			{
				return "ErrorCountByProjectVersionIndex";
			}
		}

		public class ErrorCountReduction
		{
			public string ProjectName { get; set; }

			public string ProjectVersion { get; set; }

			public int Count { get; set; }
		}
	}
}
