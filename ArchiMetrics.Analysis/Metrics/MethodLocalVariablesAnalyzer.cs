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
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class MethodLocalVariablesAnalyzer : CSharpSyntaxWalker
	{
		private int _numLocalVariables;

		public MethodLocalVariablesAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public int Calculate(SyntaxNode memberNode)
		{
			var node = memberNode as SyntaxNode;
			if (node != null)
			{
				Visit(node);
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
