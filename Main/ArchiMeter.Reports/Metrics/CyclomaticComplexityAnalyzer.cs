namespace ArchiMeter.Reports.Metrics
{
	using System.Linq;
	using Core.Metrics;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class CyclomaticComplexityAnalyzer : SyntaxWalker
	{
		private int _counter;

		private static readonly SyntaxKind[] Contributors = new[]
			                                           {
				                                           SyntaxKind.IfStatement,
				                                           SyntaxKind.WhileStatement,
				                                           SyntaxKind.ForStatement,
				                                           SyntaxKind.ForEachStatement,
				                                           SyntaxKind.CaseSwitchLabel,
				                                           SyntaxKind.DefaultExpression,
				                                           SyntaxKind.ContinueStatement,
				                                           SyntaxKind.GotoStatement,
				                                           SyntaxKind.CatchDeclaration,
				                                           SyntaxKind.CoalesceExpression,
				                                           SyntaxKind.ConditionalExpression,
				                                           SyntaxKind.LogicalAndExpression,
				                                           SyntaxKind.LogicalOrExpression,
														   SyntaxKind.LogicalNotExpression, 
			                                           };

		public CyclomaticComplexityAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
			_counter = 1;
		}

		public int Calculate(MemberNode node)
		{
			var syntax = MemberBodySelector.FindBody(node);
			if (syntax != null)
			{
				this.Visit(syntax);
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
	}
}
