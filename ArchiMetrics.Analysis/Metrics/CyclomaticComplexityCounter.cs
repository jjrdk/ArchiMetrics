// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CyclomaticComplexityCounter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CyclomaticComplexityCounter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class CyclomaticComplexityCounter
	{
		public int Calculate(MemberNode node)
		{
			var analyzer = new InnerComplexityAnalyzer();
			var result = analyzer.Calculate(node);

			return result;
		}

		private class InnerComplexityAnalyzer : SyntaxWalker
		{
			private static readonly SyntaxKind[] Contributors = new[]
																{  
																	SyntaxKind.CaseSwitchLabel, 
																	SyntaxKind.CoalesceExpression, 
																	SyntaxKind.ConditionalExpression, 
																	SyntaxKind.LogicalAndExpression, 
																	SyntaxKind.LogicalOrExpression, 
																	SyntaxKind.LogicalNotExpression
																};

			private int _counter;
			private BlockSyntax _syntax;

			public InnerComplexityAnalyzer()
				: base(SyntaxWalkerDepth.Node)
			{
				_counter = 1;
			}

			public int Calculate(MemberNode node)
			{
				_syntax = MemberBodySelector.FindBody(node);
				if (_syntax != null)
				{
					Visit(_syntax);
				}

				return _counter;
			}

			public override void Visit(SyntaxNode node)
			{
				base.Visit(node);
				if (Contributors.Contains(node.Kind))
				{
					_counter++;
				}
			}

			public override void VisitWhileStatement(WhileStatementSyntax node)
			{
				base.VisitWhileStatement(node);
				_counter++;
			}

			public override void VisitForStatement(ForStatementSyntax node)
			{
				base.VisitForStatement(node);
				_counter++;
			}

			public override void VisitForEachStatement(ForEachStatementSyntax node)
			{
				base.VisitForEachStatement(node);
				_counter++;
			}

			public override void VisitDefaultExpression(DefaultExpressionSyntax node)
			{
				base.VisitDefaultExpression(node);
				_counter++;
			}

			public override void VisitContinueStatement(ContinueStatementSyntax node)
			{
				base.VisitContinueStatement(node);
				_counter++;
			}

			public override void VisitGotoStatement(GotoStatementSyntax node)
			{
				base.VisitGotoStatement(node);
				_counter++;
			}

			public override void VisitIfStatement(IfStatementSyntax node)
			{
				base.VisitIfStatement(node);
				_counter++;
			}

			public override void VisitCatchClause(CatchClauseSyntax node)
			{
				base.VisitCatchClause(node);
				_counter++;
			}
		}
	}
}
