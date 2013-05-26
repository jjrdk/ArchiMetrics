namespace ArchiMeter.Common.Documents
{
	using System;
	using Metrics;

	public class TfsMetricsDocument
	{
		public string Id { get; set; }

		public DateTime MetricsDate { get; set; }

		public string ProjectName { get; set; }

		public string BuildNumber { get; set; }

		public NamespaceMetric[] Metrics { get; set; }

		public static string GetId(string teamProject, string revision)
		{
			return string.Format("Tfs.{0}.{1}", teamProject, revision);
		}
	}
}