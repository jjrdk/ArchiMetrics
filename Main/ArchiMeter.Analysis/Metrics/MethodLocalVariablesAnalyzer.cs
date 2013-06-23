// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodLocalVariablesAnalyzer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodLocalVariablesAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MethodLocalVariablesAnalyzer : SyntaxWalker
	{
		// Fields
		private int numLocalVariables;

		// Methods
		public MethodLocalVariablesAnalyzer()
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

			return numLocalVariables;
		}

		public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
		{
			base.VisitVariableDeclaration(node);
			numLocalVariables++;
		}
	}
}