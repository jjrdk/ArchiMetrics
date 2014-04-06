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
						new NodeReviewer(Enumerable.Empty<IEvaluation>()),
						mockRules.Object),
					new MetricsRepository(new ProjectMetricsCalculator(new CodeMetricsCalculator()), solutionProvider));
			}

			[Test, Ignore]
			public async Task CanGetVerticesForSolution()
			{
				var solutionPath = Path.GetFullPath(@"..\..\..\ArchiMetrics.sln");

				var vertices = await _repository.GetVertices(solutionPath, CancellationToken.None);

				CollectionAssert.IsNotEmpty(vertices);
			}
		}
	}
}
