// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeObfuscationRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeObfuscationRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class TypeObfuscationRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.LocalDeclarationStatement;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declaration = ((LocalDeclarationStatementSyntax)node).Declaration;

			if (declaration.Type.IsEquivalentTo(Syntax.PredefinedType(Syntax.Token(SyntaxKind.ObjectKeyword)))
				&& declaration.Variables.Any(v => v.Initializer == null || v.Initializer.Value.Kind == SyntaxKind.NullLiteralExpression))
			{
				return new EvaluationResult
						   {
							   Comment = "Type Obfuscation", 
							   Quality = CodeQuality.NeedsRefactoring, 
							   Snippet = (FindMethodParent(node) ?? FindClassParent(node)).ToFullString()
						   };
			}

			return null;
		}
	}
}
