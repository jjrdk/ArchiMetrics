// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodParameterAnalyzer.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodParameterAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MethodParameterAnalyzer : SyntaxWalker
	{
		private int numParameters;

		public MethodParameterAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(CommonSyntaxNode memberNode)
		{
			var node = memberNode as SyntaxNode;
			if (node != null)
			{
				Visit(node);
			}

			return numParameters;
		}

		public override void VisitParameter(ParameterSyntax node)
		{
			base.VisitParameter(node);
			numParameters++;
		}
	}
}