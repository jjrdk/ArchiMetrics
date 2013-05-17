namespace ArchiMeter.Reports.Metrics
{
	using Core.Metrics;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class LogicalComplexityAnalyzer : SyntaxWalker
	{
		// Fields
		private int _counter;

		// Methods
		public LogicalComplexityAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(MemberNode node)
		{
			BlockSyntax syntax = MemberBodySelector.FindBody(node);
			if (syntax != null)
			{
				this.Visit(syntax);
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
				//case SyntaxKind.EqualsExpression:
				//case SyntaxKind.NotEqualsExpression:
				case SyntaxKind.LogicalNotExpression:
					_counter++;
					break;

				case SyntaxKind.BitwiseOrExpression:
				case SyntaxKind.BitwiseAndExpression:
				case SyntaxKind.ExclusiveOrExpression:
					break;

				default:
					return;
			}
		}
	}
}