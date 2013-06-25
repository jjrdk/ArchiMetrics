// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicalComplexityAnalyzer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LogicalComplexityAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Analysis.Metrics
{
	using Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class LogicalComplexityAnalyzer : SyntaxWalker
	{
		private int _counter;

		public LogicalComplexityAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(MemberNode node)
		{
			BlockSyntax syntax = MemberBodySelector.FindBody(node);
			if (syntax != null)
			{
				Visit(syntax);
			}

			return _counter;
		}

		public override void VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			base.VisitBinaryExpression(node);
			switch (node.Kind)
			{
				case SyntaxKind.LogicalOrExpression:
				case SyntaxKind.LogicalAndExpression:
				case SyntaxKind.EqualsExpression:
				case SyntaxKind.NotEqualsExpression:
				case SyntaxKind.LogicalNotExpression:
					_counter++;
					break;
				case SyntaxKind.BitwiseOrExpression:
				case SyntaxKind.BitwiseAndExpression:
				case SyntaxKind.ExclusiveOrExpression:
					break;
			}
		}
	}
}
