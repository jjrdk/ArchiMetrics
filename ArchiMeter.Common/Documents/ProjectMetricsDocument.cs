// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsDocument.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsDocument type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Documents
{
	using Metrics;

	public class ProjectMetricsDocument
	{
		public string Id { get; set; }

		public string ProjectName { get; set; }

		public string ProjectVersion { get; set; }

		public int SourceLinesOfCode { get; set; }

		public NamespaceMetric[] Metrics { get; set; }

		public static string GetId(string project, string revision)
		{
			return string.Format("Metric.{0}.{1}", project, revision);
		}
	}
}