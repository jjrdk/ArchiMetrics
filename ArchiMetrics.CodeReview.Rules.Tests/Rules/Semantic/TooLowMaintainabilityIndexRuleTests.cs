// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooLowMaintainabilityIndexRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooLowMaintainabilityIndexRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using NUnit.Framework;
	

	public sealed class TooLowMaintainabilityIndexRuleTests
	{
		private TooLowMaintainabilityIndexRuleTests()
		{
		}

		public class GivenATooLowMaintainabilityIndexRule : SolutionTestsBase
		{
			private const string HighMaintainability = @"public class MyClass
{
	public void DoSomething()
	{
		System.Console.WriteLine(""Hello World"");
	}
}";

			private TooLowMaintainabilityIndexRule _rule;
			
			[SetUp]
			public void Setup()
			{
				_rule = new TooLowMaintainabilityIndexRule();
			}

			[Test]
			public async Task WhenMethodHasHighMaintainabilityThenReturnsNull()
			{
				var solution = CreateSolution(HighMaintainability);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModelAsync().Result
										let root = d.GetSyntaxRootAsync().Result
										from n in root.DescendantNodes().OfType<MethodDeclarationSyntax>()
										select new
										{
											semanticModel = model, 
											node = n
										})
										.First();
				var result = await _rule.Evaluate(classDeclaration.node, classDeclaration.semanticModel, solution);

				Assert.Null(result);
			}
		}
	}
}