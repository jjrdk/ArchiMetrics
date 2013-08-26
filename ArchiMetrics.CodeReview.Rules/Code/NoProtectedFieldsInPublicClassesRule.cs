// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoProtectedFieldsInPublicClassesRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoProtectedFieldsInPublicClassesRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class NoProtectedFieldsInPublicClassesRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.FieldDeclaration;
			}
		}
		public override string Title
		{
			get
			{
				return "No Protected Fields";
			}
		}
		public override string Suggestion
		{
			get
			{
				return "Encapsulate all public fields in properties, or internalize them.";
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
							       Quality = CodeQuality.Broken, 
								   QualityAttribute = QualityAttribute.Modifiability, 
							       Snippet = classParent.ToFullString(),
								   ImpactLevel = ImpactLevel.Type
						       };
				}
			}

			return null;
		}
	}
}
