// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeReviewerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeReviewerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using System.Threading;

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Analysis.Common.CodeReview;
    using ArchiMetrics.Analysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    public sealed class NodeReviewerTests : SolutionTestsBase
    {
        private NodeReviewerTests()
        {
        }

        public static async Task<IEnumerable<EvaluationResult>> PerformInspection(string code, Type evaluatorType)
        {
            var inspector = new NodeReviewer(new[] { (ICodeEvaluation)Activator.CreateInstance(evaluatorType) }, Enumerable.Empty<ISymbolEvaluation>());
            var tree = CSharpSyntaxTree.ParseText("namespace TestSpace { public class ParseClass { " + code + " } }", CSharpParseOptions.Default, string.Empty, Encoding.Unicode, CancellationToken.None);

            return await inspector.Inspect(string.Empty, string.Empty, tree.GetRoot(), null, null);
        }

        public static Task<IEnumerable<EvaluationResult>> PerformSolutionInspection(string code, Type evaluatorType)
        {
            var inspector = new NodeReviewer(new[] { (ICodeEvaluation)Activator.CreateInstance(evaluatorType) }, Enumerable.Empty<ISymbolEvaluation>());
            code = "namespace TestSpace { public class ParseClass { " + code + " } }";

            var solution = CreateSolution(code);
            var task = inspector.Inspect(solution);
            return task;
        }

        public class GivenANodeInspectorInspectingBrokenSolution
        {
            [Theory]
            [ClassData(typeof(BrokenCode))]
            public async Task WhenInspectingSolutionThenFindsErrors(string code, Type evaluatorType)
            {
                Console.WriteLine(evaluatorType.Name);

                var task = await PerformSolutionInspection(code, evaluatorType);
                var count = task.Count();

                Assert.Equal(1, count);
            }
        }

        public class GivenANodeReviewerInspectingBrokenCode
        {
            [Theory]
            [ClassData(typeof(BrokenCode))]
            public async Task SyntaxDetectionTest(string code, Type evaluatorType)
            {
                var task = await PerformInspection(code, evaluatorType);
                var count = task.Count();

                Assert.Equal(1, count);
            }

            [Theory]
            [ClassData(typeof(BrokenCode))]
            public async Task WhenCreatingResultThenIncludesTypeName(string code, Type evaluatorType)
            {
                var result = await PerformInspection(code, evaluatorType);

                Assert.True(result.All(x => !string.IsNullOrWhiteSpace(x.TypeName)));
            }

            [Theory]
            [ClassData(typeof(BrokenCode))]
            public async Task WhenCreatingResultThenIncludesNamespaceName(string code, Type evaluatorType)
            {
                var result = await PerformInspection(code, evaluatorType);

                Assert.True(
                    result.All(
                        x => !string.IsNullOrWhiteSpace(x.Namespace) && x.Namespace != SyntaxFactory.Token(SyntaxKind.GlobalKeyword)
                                                                                           .ValueText));
            }
        }

        public class GivenANodeReviewerInspectingNonBrokenCode
        {
            [Theory]
            [ClassData(typeof(WorkingCode))]
            public async Task NegativeTest(string code, Type evaluatorType)
            {
                var result = await PerformInspection(code, evaluatorType);

                Assert.Empty(result);
            }
        }
    }
}