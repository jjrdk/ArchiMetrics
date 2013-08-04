// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataComparer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectDataComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview
{
	using System.Collections.Generic;

	public class ProjectDataComparer : IEqualityComparer<ProjectData>
	{
		private static readonly ProjectDataComparer SingleInstance = new ProjectDataComparer();

		private ProjectDataComparer()
		{
		}

		public static ProjectDataComparer Instance
		{
			get
			{
				return SingleInstance;
			}
		}

		public bool Equals(ProjectData x, ProjectData y)
		{
			return x == null
					   ? y == null
					   : y != null && string.Equals(x.ProjectName, y.ProjectName)
						 && string.Equals(x.ProjectVersion, y.ProjectVersion);
		}

		public int GetHashCode(ProjectData obj)
		{
			return (obj.ProjectName + obj.ProjectVersion).GetHashCode();
		}
	}
}
