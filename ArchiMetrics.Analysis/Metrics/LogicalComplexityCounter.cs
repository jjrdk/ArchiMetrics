// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicalComplexityCounter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LogicalComplexityCounter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class LogicalComplexityCounter : SyntaxWalker
	{
		private int _counter;

		public LogicalComplexityCounter()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(MemberNode node)
		{
			var syntax = node.SyntaxNode as SyntaxNode;
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
