// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionVertexRepositoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionVertexRepositoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Model
{
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Analysis.Model;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Moq;
	using NUnit.Framework;

	public sealed class SolutionVertexRepositoryTests
	{
		private SolutionVertexRepositoryTests()
		{
		}

		public class GivenASolutionVertexRepository
		{
			private SolutionVertexRepository _repository;

			[SetUp]
			public void Setup()
			{
				var mockRules = new Mock<IAvailableRules>();
				mockRules.Setup(x => x.GetEnumerator())
					.Returns(Enumerable.Empty<IEvaluation>().GetEnumerator());
				var solutionProvider = new SolutionProvider();
				_repository = new SolutionVertexRepository(
					new CodeErrorRepository(
						solutionProvider,
						new NodeReviewer(Enumerable.Empty<IEvaluation>(), Enumerable.Empty<ISymbolEvaluation>()), 
						mockRules.Object),
					new MetricsRepository(new ProjectMetricsCalculator(new CodeMetricsCalculator(new TypeDocumentationFactory(), new MemberDocumentationFactory())), solutionProvider));
			}

			[Test]
			[Ignore("Not implemented yet.")]
			public async Task CanGetVerticesForSolution()
			{
				var solutionPath = @"..\..\..\ArchiMetrics.sln".GetLowerCaseFullPath();

				var vertices = await _repository.GetVertices(solutionPath, CancellationToken.None);

				CollectionAssert.IsNotEmpty(vertices);
			}
		}
	}
}
