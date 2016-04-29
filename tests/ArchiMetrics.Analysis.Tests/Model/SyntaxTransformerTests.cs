// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxTransformerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxTransformerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.Model;
    using Common.CodeReview;
    using Common.Structure;
    using Xunit;

    public sealed class SyntaxTransformerTests
    {
        private SyntaxTransformerTests()
        {
        }

        public class GivenASyntaxTransformer
        {
            private readonly SyntaxTransformer _transformer;

            public GivenASyntaxTransformer()
            {
                _transformer = new SyntaxTransformer();
            }

            [Fact]
            public async Task WhenTransformingThenTransformsDisplayName()
            {
                var rule = new TransformRule("Test", "[xyz]");
                var node = new ModelNode("x", "type", CodeQuality.Good, 3, 2, 1);

                var transformed = await _transformer.Transform(new[] { node }, new[] { rule }, CancellationToken.None);

                Assert.Equal("Test", transformed.First().DisplayName);
            }

            [Fact]
            public async Task WhenTransformingThenAlsoTransformsChildren()
            {
                var rule = new TransformRule("Test", "[xyz]");
                var node = new ModelNode("a", "type", CodeQuality.Good, 3, 2, 1, new List<IModelNode> { new ModelNode("x", "type", CodeQuality.Good, 6, 5, 4) });

                var transformed = await _transformer.Transform(new[] { node }, new[] { rule }, CancellationToken.None);

                Assert.Equal("Test", transformed.First().Children.First().DisplayName);
            }
        }
    }
}
