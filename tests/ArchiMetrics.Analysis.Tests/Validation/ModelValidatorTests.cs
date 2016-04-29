// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelValidatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelValidatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Validation
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.Model;
    using ArchiMetrics.Analysis.Validation;
    using Common;
    using Common.CodeReview;
    using Common.Structure;
    using Moq;
    using Xunit;

    public sealed class ModelValidatorTests
    {
        private ModelValidatorTests()
        {
        }

        public class GivenAModelValidator
        {
            private readonly ModelValidator _validator;

            public GivenAModelValidator()
            {
                var mockSyntaxTransformer = new Mock<ISyntaxTransformer>();
                var mockVertexRepository = new Mock<IVertexRepository>();

                _validator = new ModelValidator(mockSyntaxTransformer.Object, mockVertexRepository.Object);
            }

            [Fact]
            public async Task WhenValidationRulesArePassedThenResultNotEmpty()
            {
                var solutionPath = @"..\..\..\ArchiMetrics.sln".GetLowerCaseFullPath();
                var rule = new BranchModelRule(new ModelNode("ArchiMetrics.Analysis", NodeKind.Assembly, CodeQuality.Good, 0, 0, 0));
                var result = await _validator.Validate(solutionPath, new[] { rule }, Enumerable.Empty<TransformRule>(), CancellationToken.None);

                Assert.NotEmpty(result);
            }
        }
    }
}
