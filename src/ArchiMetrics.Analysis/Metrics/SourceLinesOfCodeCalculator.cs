namespace ArchiMetrics.Analysis.Metrics
{
    using System;
    using Microsoft.CodeAnalysis;

    internal sealed class SourceLinesOfCodeCalculator
    {
        public int Calculate(SyntaxNode node)
            => node.ToFullString().Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}