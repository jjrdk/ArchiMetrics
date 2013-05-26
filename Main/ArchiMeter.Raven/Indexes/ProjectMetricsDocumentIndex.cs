// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsDocumentIndex.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsDocumentIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Abstractions.Indexing;
	using global::Raven.Client.Indexes;

	public class ProjectMetricsDocumentIndex : AbstractIndexCreationTask<ProjectMetricsDocument>
	{
		public ProjectMetricsDocumentIndex()
		{
			Map = ed => from e in ed
			                 select new
				                        {
					                        e.ProjectName, 
					                        e.ProjectVersion, 
											e.SourceLinesOfCode
				                        };

			Store(e => e.ProjectName, FieldStorage.Yes);
			Store(e => e.ProjectVersion, FieldStorage.Yes);
		}
	}
}