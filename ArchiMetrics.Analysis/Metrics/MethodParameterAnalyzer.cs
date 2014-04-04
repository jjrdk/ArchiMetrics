// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodParameterAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class MethodParameterAnalyzer : CSharpSyntaxWalker
	{
		private int _numberOfParameters;

		public MethodParameterAnalyzer()
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

			return _numberOfParameters;
		}

		public override void VisitParameter(ParameterSyntax node)
		{
			base.VisitParameter(node);
			_numberOfParameters++;
		}
	}
}
