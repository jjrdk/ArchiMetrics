namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Code
{
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.CodeReview.Rules.Code;
	using NUnit.Framework;

	public sealed class TypeMustBeDeclaredInNamespaceRuleTests
	{
		private TypeMustBeDeclaredInNamespaceRuleTests()
		{
		}

		public class GivenATypeMustBeDeclaredInNamespaceRule : SolutionTestsBase
		{
			private NodeReviewer _inspector;

			[SetUp]
			public void Setup()
			{
				_inspector = new NodeReviewer(new[] { new TypeMustBeDeclaredInNamespaceRule() });
			}

			[Test]
			public async Task WhenTypeIsDeclaredOutsideNamespaceThenReturnsError()
			{
				const string Code = @"public class MyClass
{
	public string Value { get; set; }
}";

				var solution = CreateSolution(Code);

				var results = await _inspector.Inspect(solution);

				CollectionAssert.IsNotEmpty(results);
			}
		}
	}
}