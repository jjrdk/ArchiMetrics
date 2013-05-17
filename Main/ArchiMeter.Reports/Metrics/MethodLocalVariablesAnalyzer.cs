namespace ArchiMeter.Reports.Metrics
{
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class MethodLocalVariablesAnalyzer : SyntaxWalker
	{
		private int _numLocalVariables;

		public MethodLocalVariablesAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(CommonSyntaxNode memberNode)
		{
			SyntaxNode node = memberNode as SyntaxNode;
			if (node != null)
			{
				this.Visit(node);
			}
			return _numLocalVariables;
		}

		public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
		{
			base.VisitVariableDeclaration(node);
			_numLocalVariables++;
		}
	}
}