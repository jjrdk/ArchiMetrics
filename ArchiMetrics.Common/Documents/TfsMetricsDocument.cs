namespace ArchiMetrics.Common.Documents
{
	using System;
	using Metrics;

	public class TfsMetricsDocument : ProjectDocument
	{
		public string Id { get; set; }

		public DateTime MetricsDate { get; set; }

		public string BuildNumber { get; set; }

		public NamespaceMetric[] Metrics { get; set; }

		public static string GetId(string teamProject, string revision, string buildNo)
		{
			return string.Format("Tfs.{0}.{1}.{2}", teamProject, revision, buildNo);
		}
	}
}
