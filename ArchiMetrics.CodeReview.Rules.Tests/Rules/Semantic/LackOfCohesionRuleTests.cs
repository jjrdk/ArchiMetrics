// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LackOfCohesionRuleTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LackOfCohesionRuleTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules.Semantic
{
	using System.Linq;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using NUnit.Framework;

	public sealed class LackOfCohesionRuleTests
	{
		private LackOfCohesionRuleTests()
		{
		}

		public class GivenALackOfCohesionOfMethodsRule : SolutionTestsBase
		{
			private const string Uncohesive = @"namespace MyNamespace
{
	public class MyClass
	{
		private int _field1 = 1;
		private int _field2 = 2;
		private int _field3 = 3;
		private int _field4 = 4;
		private int _field5 = 5;
		private int _field6 = 6;
		private int _field7 = 7;
		private int _field8 = 8;
		private int _field9 = 9;
		private int _field10 = 10;
		private int _field11 = 11;

		public int GetNumber1()
		{
			return _field1;
		}

		public int GetNumber2()
		{
			return _field2;
		}

		public int GetNumber3()
		{
			return _field3;
		}

		public int GetNumber4()
		{
			return _field4;
		}

		public int GetNumber5()
		{
			return _field5;
		}

		public int GetNumber6()
		{
			return _field6;
		}

		public int GetNumber7()
		{
			return _field7;
		}

		public int GetNumber8()
		{
			return _field8;
		}

		public int GetNumber9()
		{
			return _field9;
		}

		public int GetNumber10()
		{
			return _field10;
		}

		public int GetNumber11()
		{
			return _field11;
		}
	}
}";

			private LackOfCohesionOfMethodsRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new LackOfCohesionOfMethodsRule();
			}

			[Test]
			public void WhenAnalyzingAnUncohesiveClassThenReturnsError()
			{
				var solution = CreateSolution(Uncohesive);
				var classDeclaration = (from p in solution.Projects
										from d in p.Documents
										let model = d.GetSemanticModelAsync().Result
										let root = d.GetSyntaxRootAsync().Result
										from n in root.DescendantNodes().OfType<ClassDeclarationSyntax>()
										select new
											   {
												   semanticModel = model, 
												   node = n
											   }).First();
				var result = _rule.Evaluate(classDeclaration.node, classDeclaration.semanticModel, solution);

				Assert.NotNull(result);
			}
		}
	}
}