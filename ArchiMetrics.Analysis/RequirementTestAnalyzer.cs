namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class RequirementTestAnalyzer : IRequirementTestAnalyzer
	{
		private static readonly string[] TestNames = { "TestMethod", "Test", "TestCase", "Fact" };
		private readonly IProvider<string, IProject> _provider;

		public RequirementTestAnalyzer(IProvider<string, IProject> solutionProvider)
		{
			_provider = solutionProvider;
		}

		public IEnumerable<TestData> GetTestData(string path)
		{
			var tests = GetTests(path).ToArray();
			return tests.Select(GetRequirementsForTest);
		}

		public IEnumerable<RequirementToTestReport> GetRequirementTests(string path)
		{
			var testData = GetTestData(path).ToArray();
			var allRequirements = testData.SelectMany(d => d.RequirementIds).Distinct();
			return allRequirements.Select(r => new RequirementToTestReport(r, testData.Where(d => d.RequirementIds.Any(i => i == r))));
		}

		private static bool IsTestMethod(MethodDeclarationSyntax syntax)
		{
			return syntax
				.AttributeLists
				.Any(l => l.Attributes
					.Select(a=>a.Name)
					.OfType<SimpleNameSyntax>()
					.Any(s => TestNames.Any(t => s.Identifier.ValueText == t)));
		}

		private IEnumerable<MethodDeclarationSyntax> GetTests(string tests)
		{
			var projects = _provider.GetAll(tests).ToArray();
			return projects
				.Where(p => p != null)
				.Distinct(ProjectComparer.Default)
				.SelectMany(p => p.Documents)
				.Select(d => d.GetSyntaxRoot() as SyntaxNode)
				.Where(n => n != null)
				.SelectMany(n => n.DescendantNodes(x => true))
				.Where(n => n.Kind == SyntaxKind.MethodDeclaration)
				.Cast<MethodDeclarationSyntax>()
				.Where(IsTestMethod)
				.ToArray();
		}

		private static int GetAssertCount(MethodDeclarationSyntax node)
		{
			var descendants = node.DescendantNodes(n => true).ToArray();
			var asserts = descendants
				.Where(n => n.Kind == SyntaxKind.MemberAccessExpression)
				.OfType<MemberAccessExpressionSyntax>()
				.Select(n => n.Expression)
				.OfType<SimpleNameSyntax>()
				.Count(n => n.Identifier.ValueText == "Assert" || n.Identifier.ValueText == "ExceptionAssert");
			var mockCount = descendants
				.Where(n => n.Kind == SyntaxKind.MemberAccessExpression)
				.OfType<MemberAccessExpressionSyntax>()
				.Count(n => n.Name.Identifier.ValueText == "Verify" || n.Name.Identifier.ValueText == "VerifySet" || n.Name.Identifier.ValueText == "VerifyGet");
			var expectationCount = node
				.AttributeLists
				.Count(l => l.Attributes.Select(a => a.Name).OfType<SimpleNameSyntax>().Any(n => n.Identifier.ValueText == "ExpectedException"));
			return asserts + mockCount + expectationCount;
		}

		private static AttributeSyntax GetTestProperties(SyntaxNode node)
		{
			var attributeSyntax = node as AttributeSyntax;
			return attributeSyntax != null && attributeSyntax.Name.ToFullString() == "TestProperty"
					   ? attributeSyntax
					   : null;
		}

		private static IEnumerable<string> GetRequirementsProperties(AttributeSyntax node)
		{
			if (node.ArgumentList != null && node.ArgumentList.Arguments.Any(a => a.Expression.ToFullString() == "TC.Requirement"))
			{
				return node.ArgumentList.Arguments
						   .Where(a => a.Expression.ToFullString() != "TC.Requirement")
						   .Select(a => a.ToFullString().Trim('"'))
						   .SelectMany(s => s.Split(','))
						   .Select(s => s.Trim())
						   .Select(s => s == "" ? "0" : s);
			}

			return Enumerable.Empty<string>();
		}

		private static TestData GetRequirementsForTest(MethodDeclarationSyntax node)
		{
			var children = node.ChildNodes();
			var reqAttribute = children
				.Where(n => n.Kind == SyntaxKind.AttributeList)
				.Cast<AttributeListSyntax>()
				.SelectMany(n => n.ChildNodes().Select(GetTestProperties))
				.Where(a => a != null)
				.SelectMany(GetRequirementsProperties)
				.Select(s => Convert.ToInt32(s));
			var assertCount = GetAssertCount(node);
			return new TestData(reqAttribute, assertCount, node.Identifier.ValueText, node.ToFullString());
		}
	}
}
