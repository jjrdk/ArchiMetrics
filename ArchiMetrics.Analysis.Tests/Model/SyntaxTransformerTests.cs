namespace ArchiMetrics.Analysis.Tests.Model
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Model;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;
	using NUnit.Framework;

	public sealed class SyntaxTransformerTests
	{
		private SyntaxTransformerTests()
		{
		}

		public class GivenASyntaxTransformer
		{
			private SyntaxTransformer _transformer;

			[SetUp]
			public void Setup()
			{
				_transformer = new SyntaxTransformer();
			}

			[Test]
			public async Task WhenTransformingThenTransformsDisplayName()
			{
				var rule = new TransformRule
						   {
							   Name = "Test",
							   Pattern = "[xyz]"
						   };
				var node = new ModelNode("x", "type", CodeQuality.Good, 3, 2, 1);

				var transformed = await _transformer.Transform(new[] { node }, new[] { rule }, CancellationToken.None);

				Assert.AreEqual("Test", transformed.First().DisplayName);
			}

			[Test]
			public async Task WhenTransformingThenAlsoTransformsChildren()
			{
				var rule = new TransformRule
						   {
							   Name = "Test",
							   Pattern = "[xyz]"
						   };
				var node = new ModelNode("a", "type", CodeQuality.Good, 3, 2, 1, new List<IModelNode> { new ModelNode("x", "type", CodeQuality.Good, 6, 5, 4) });

				var transformed = await _transformer.Transform(new[] { node }, new[] { rule }, CancellationToken.None);

				Assert.AreEqual("Test", transformed.First().Children.First().DisplayName);
			}
		}
	}
}
