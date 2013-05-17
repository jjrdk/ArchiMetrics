// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectVersionView.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectVersionView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Indexes
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using CodeReview;
	using global::Raven.Database.Linq;

	[DisplayName("Compiled/ProjectVersion")]
	public class ProjectVersionView : AbstractViewGenerator
	{
		public ProjectVersionView()
		{
			AddMapDefinition(docs => docs.Select(document =>
				{
					try
					{
						return new ProjectData
							       {
								       ProjectName = document.ProjectName, 
								       ProjectVersion = document.ProjectVersion
							       };
					}
					catch
					{
						return null;
					}
				})
			                             .Where(d => d != null));

			GroupByExtraction = source => source.ProjectName + source.ProjectVersion;
			ReduceDefinition = Reduce;

			AddField("ProjectName");
			AddField("ProjectVersion");
		}

		private static IEnumerable<ProjectData> Reduce(IEnumerable<dynamic> source)
		{
			return source.OfType<ProjectData>().Distinct(ProjectDataComparer.Instance);
		}
	}
}