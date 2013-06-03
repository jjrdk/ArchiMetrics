// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyTestRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyTestRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class EmptyTestRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var methodParent = (MethodDeclarationSyntax)node;

			if (methodParent != null
				&& methodParent.AttributeLists != null
				&& methodParent.AttributeLists.Any(
					l => l.Attributes.Any(a => a.Name is SimpleNameSyntax
											   && ((SimpleNameSyntax)a.Name).Identifier.ValueText.IsKnownTestAttribute())))
			{
				if (methodParent.Body == null
					|| !methodParent.Body.ChildNodes().Any())
				{
					return new EvaluationResult
						   {
							   Comment = "Empty test found", 
							   ImpactLevel = ImpactLevel.Type, 
							   Quality = CodeQuality.NeedsReview, 
							   QualityAttribute = QualityAttribute.Testability, 
							   Snippet = (FindClassParent(node) ?? node).ToFullString()
						   };
				}
			}

			return null;
		}
	}
}
