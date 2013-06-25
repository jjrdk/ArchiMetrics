// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInventoryDocumentIndex.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectInventoryDocumentIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Abstractions.Indexing;
	using global::Raven.Client.Indexes;

	public class ProjectInventoryDocumentIndex : AbstractIndexCreationTask<ProjectInventoryDocument>
	{
		public ProjectInventoryDocumentIndex()
		{
			Map = ed => from e in ed
			            select new
				                   {
					                   e.ProjectName, 
					                   e.ProjectVersion
				                   };

			Store(e => e.ProjectName, FieldStorage.Yes);
			Store(e => e.ProjectVersion, FieldStorage.Yes);
		}
	}
}
