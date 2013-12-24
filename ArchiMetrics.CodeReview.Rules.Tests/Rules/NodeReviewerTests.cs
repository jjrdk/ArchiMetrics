// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeReviewerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeReviewerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common.CodeReview;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public sealed class NodeReviewerTests
	{
		private NodeReviewerTests()
		{
		}

		private static Task<IEnumerable<EvaluationResult>> PerformInspection(string code, Type evaluatorType)
		{
			var inspector = new NodeReviewer(new[] { (ICodeEvaluation)Activator.CreateInstance(evaluatorType) });
			var tree = SyntaxTree.ParseText("namespace TestSpace { public class ParseClass { " + code + " } }");

			var task = inspector.Inspect(string.Empty, tree.GetRoot(), null, null);
			return task;
		}

		public class GivenANodeReviewerInspectingBrokenCode
		{
			[TestCaseSource(typeof(InspectionCodeSource), "BrokenCode")]
			public void SyntaxDetectionTest(string code, Type evaluatorType)
			{
				var task = PerformInspection(code, evaluatorType);
				var count = task.Result.Count();

				Assert.AreEqual(1, count);
			}

			[TestCaseSource(typeof(InspectionCodeSource), "BrokenCode")]
			public async Task WhenCreatingResultThenIncludesTypeName(string code, Type evaluatorType)
			{
				var result = await PerformInspection(code, evaluatorType);

				Assert.IsTrue(result.All(x => !string.IsNullOrWhiteSpace(x.TypeName)));
			}

			[TestCaseSource(typeof(InspectionCodeSource), "BrokenCode")]
			public async Task WhenCreatingResultThenIncludesNamespaceName(string code, Type evaluatorType)
			{
				var result = await PerformInspection(code, evaluatorType);

				Assert.IsTrue(result.All(x => !string.IsNullOrWhiteSpace(x.Namespace) && x.Namespace != Syntax.Token(SyntaxKind.GlobalKeyword).ValueText));
			}
		}

		public class GivenASyntaxInspectorInspectingNonBrokenCode
		{
			[TestCaseSource(typeof(InspectionCodeSource), "WrokingCode")]
			public void NegativeTest(string code, Type evaluatorType)
			{
				var task = PerformInspection(code, evaluatorType);
				task.Wait();
				Assert.IsEmpty(task.Result);
			}
		}
	}
}
