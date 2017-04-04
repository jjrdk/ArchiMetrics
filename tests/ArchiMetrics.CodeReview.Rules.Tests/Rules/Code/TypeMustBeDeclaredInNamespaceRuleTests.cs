// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMustBeDeclaredInNamespaceRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMustBeDeclaredInNamespaceRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Code
{
    using System.Linq;
    using System.Threading.Tasks;
    using Analysis;
    using Analysis.Common.CodeReview;
    using ArchiMetrics.CodeReview.Rules.Code;
    using Xunit;

    public sealed class TypeMustBeDeclaredInNamespaceRuleTests
    {
        private TypeMustBeDeclaredInNamespaceRuleTests()
        {
        }

        public class GivenATypeMustBeDeclaredInNamespaceRule : SolutionTestsBase
        {
            private readonly NodeReviewer _inspector;

            public GivenATypeMustBeDeclaredInNamespaceRule()
            {
                _inspector = new NodeReviewer(new[] { new TypeMustBeDeclaredInNamespaceRule() }, Enumerable.Empty<ISymbolEvaluation>());
            }

            [Fact]
            public async Task WhenTypeIsDeclaredOutsideNamespaceThenReturnsError()
            {
                const string Code = @"public class MyClass
{
	public string Value { get; set; }
}";

                var solution = CreateSolution(Code);

                var results = await _inspector.Inspect(solution);

                Assert.NotEmpty(results);
            }
        }
    }
}