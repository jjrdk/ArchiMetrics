// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

			var metric = new ProjectMetric("name", mockNamespaces.Object, Enumerable.Empty<string>(), Enumerable.Empty<string>(), 0);

			mockNamespaces.Verify(x => x.GetEnumerator(), Times.AtLeastOnce);
		}
	}
}