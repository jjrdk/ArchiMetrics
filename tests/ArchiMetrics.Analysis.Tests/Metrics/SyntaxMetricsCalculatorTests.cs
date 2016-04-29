// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxMetricsCalculatorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SyntaxMetricsCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.Metrics
{
    using System.Linq;
    using ArchiMetrics.Analysis.Metrics;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    public class SyntaxMetricsCalculatorTests
    {
        [Fact]
        public void CanGetHalsteadMetricsForClassSnippet()
        {
            const string Code = @"public class Something {
public int Number { get{ return a - b; } }
public string GetValue() { return ""x"" + a; }
}";
            var root = CSharpSyntaxTree.ParseText(Code).GetRoot();

            var metrics = new SyntaxMetricsCalculator().Calculate(root);

            Assert.NotNull(metrics);
        }

        [Fact]
        public void CanGetHalsteadMetricsForMethodSnippet()
        {
            const string Code = @"public string GetValue() { return ""x""; }";
            var root = CSharpSyntaxTree.ParseText(Code).GetRoot();

            var metrics = new SyntaxMetricsCalculator().Calculate(root);

            Assert.NotNull(metrics);
        }

        [Fact]
        public void CanGetHalsteadMetricsForArbitrarySnippet()
        {
            const string Code = @"return ""x"";";
            var root = CSharpSyntaxTree.ParseText(Code).GetRoot();

            var metrics = new SyntaxMetricsCalculator().Calculate(root);

            Assert.NotNull(metrics);
        }

        [Fact]
        public void CanGetHalsteadMetricsForMultipleArbitrarySnippets()
        {
            const string Code = @"var a = ""x""; return a;";
            var root = CSharpSyntaxTree.ParseText(Code).GetRoot();

            var metrics = new SyntaxMetricsCalculator().Calculate(root);

            Assert.NotNull(metrics);
        }

        [Theory]
        [InlineData(@"public class Something {
public int Number { get{ return a - b; } }
public string GetValue() { return ""x"" + a; }
}")]
        [InlineData(@"return ""x"";")]
        [InlineData(@"var a = ""x""; return a;")]
        public void CodeHasEffort(string code)
        {
            var metrics = new SyntaxMetricsCalculator().Calculate(code);

            var effort = metrics.Sum(m => m.GetEffort().TotalSeconds * 18);
            Assert.True(effort > 0);
        }
    }
}
