// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodLocalVariablesAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodLocalVariablesAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MethodLocalVariablesAnalyzer : SyntaxWalker
	{
		private int numLocalVariables;

		public MethodLocalVariablesAnalyzer()
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

			return numLocalVariables;
		}

		public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
		{
			base.VisitVariableDeclaration(node);
			numLocalVariables++;
		}
	}
}
