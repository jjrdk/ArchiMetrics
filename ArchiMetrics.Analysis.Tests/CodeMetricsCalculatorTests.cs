// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeMetricsCalculatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeMetricsCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis.Metrics;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public sealed class CodeMetricsCalculatorTests
    {
        private CodeMetricsCalculatorTests()
        {
        }

        public class GivenACodeMetricsCalculator
        {
            private CodeMetricsCalculator _calculator;

            [SetUp]
            public void Setup()
            {
                _calculator = new CodeMetricsCalculator(new TypeDocumentationFactory(), new MemberDocumentationFactory());
            }

            [Test]
            public async Task WhenCalculatingClassCouplingThenReturnsCorrectCount()
            {
                var snippet = @"
using System;
using system.Diagnostics;

namespace Metric.Test
{
    public class Testmetricclass
    {
        public void TestClassCoupling()
        { 
            Console.WriteLine(""Hello world"");
            Trace.WriteLine(""This method uses Console and Trace"");
        }
    }
}";

                var tree = CSharpSyntaxTree.ParseText(snippet);
                var metrics = await _calculator.Calculate(new[] { tree });

	            var actual = metrics.First().ClassCouplings.Count();
	            Assert.AreEqual(2, actual);
            }

            [Test]
            public void WhenCalculatingMetricsForCodeSnippetThenReturnsMetrics()
            {
                var snippet = @"public int GetValue(int x)
{
	if(x% 2 == 0)
	{
		return x-2;
	}

	return x;
}";
                var tree = CSharpSyntaxTree.ParseText(snippet);
                var metrics = _calculator.Calculate(new[] { tree });

                Assert.NotNull(metrics);
            }
        }
    }
}
