namespace ArchiMetrics.Analysis.Tests
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;
	using Moq;
	using NUnit.Framework;

	public class ProjectMetricTests
	{
		[Test]
		public void WhenCreatingProjectMetricThenEnumeratesPassedNamespaceMetrics()
		{
			var mockNamespaces = new Mock<IEnumerable<INamespaceMetric>>();
			mockNamespaces.Setup(x => x.GetEnumerator()).Returns(Enumerable.Empty<INamespaceMetric>().GetEnumerator());

			var metric = new ProjectMetric("name", mockNamespaces.Object, Enumerable.Empty<string>());

			mockNamespaces.Verify(x => x.GetEnumerator(), Times.AtLeastOnce);
		}
	}
}