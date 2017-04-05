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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.Metrics;
    using ArchiMetrics.Analysis.Model;
    using Common;
    using Common.CodeReview;
    using Moq;
    using Xunit;

    public sealed class SolutionVertexRepositoryTests
    {
        private SolutionVertexRepositoryTests()
        {
        }

        public class GivenASolutionVertexRepository
        {
            private readonly SolutionVertexRepository _repository;

            public GivenASolutionVertexRepository()
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

            [Fact(Skip = "Not implemented yet.")]
            public async Task CanGetVerticesForSolution()
            {
                var solutionPath = @"..\..\..\ArchiMetrics.sln".GetLowerCaseFullPath();

                var vertices = await _repository.GetVertices(solutionPath, CancellationToken.None).ConfigureAwait(false);

                Assert.NotEmpty(vertices);
            }
        }
    }
}
