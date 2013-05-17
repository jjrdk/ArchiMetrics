// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodParameterAnalyzer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodParameterAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class MethodParameterAnalyzer : SyntaxWalker
	{
		// Fields
		private int numParameters;

		// Methods
		public MethodParameterAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(CommonSyntaxNode memberNode)
		{
			//////Verify.NotNull<CommonSyntaxNode>(Expression.Lambda<Func<CommonSyntaxNode>>(Expression.Constant(memberNode), new ParameterExpression[0]), (string)null);
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