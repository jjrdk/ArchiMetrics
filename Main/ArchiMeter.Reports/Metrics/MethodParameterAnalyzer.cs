namespace ArchiMeter.Reports.Metrics
{
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class MethodParameterAnalyzer : SyntaxWalker
	{
		private int _numParameters;

		public MethodParameterAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(CommonSyntaxNode memberNode)
		{
			var node = memberNode as SyntaxNode;
			if (node != null)
			{
				this.Visit(node);
			}
			return _numParameters;
		}

		public override void VisitParameter(ParameterSyntax node)
		{
			base.VisitParameter(node);
			_numParameters++;
		}
	}
}