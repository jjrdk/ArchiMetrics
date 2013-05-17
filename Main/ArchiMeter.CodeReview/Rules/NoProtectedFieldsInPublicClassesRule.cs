// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoProtectedFieldsInPublicClassesRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoProtectedFieldsInPublicClassesRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class NoProtectedFieldsInPublicClassesRule : EvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.FieldDeclaration;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var classParent = FindClassParent(node);
			if (classParent != null && classParent.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				var syntax = (FieldDeclarationSyntax)node;
				if (syntax.Modifiers.Any(SyntaxKind.ProtectedKeyword))
				{
					return new EvaluationResult
						       {
							       Comment = "Protected field declaration in public class", 
							       Quality = CodeQuality.Broken, 
								   QualityAttribute = QualityAttribute.Modifiability, 
							       Snippet = classParent.ToFullString()
						       };
				}
			}

			return null;
		}
	}
}