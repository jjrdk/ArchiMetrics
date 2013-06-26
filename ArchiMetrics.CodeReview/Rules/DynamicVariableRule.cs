// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicVariableRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicVariableRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class DynamicVariableRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.VariableDeclaration;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var variableDeclaration = (VariableDeclarationSyntax)node;
			if (variableDeclaration.Type.GetText().ToString().Trim() == "dynamic")
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent.ToFullString();

				return new EvaluationResult
					       {
						       Comment = "Use of dynamic variable.", 
						       Quality = CodeQuality.Broken, 
							   QualityAttribute = QualityAttribute.Conformance, 
						       Snippet = snippet
					       };
			}

			return null;
		}
	}
}
