// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalTimeCreationRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LocalTimeCreationRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class LocalTimeCreationRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MemberAccessExpression;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.Kind == SyntaxKind.IdentifierName
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "DateTime"
				&& memberAccess.Name.Identifier.ValueText == "Now")
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent == null
								  ? node.ToFullString()
								  : methodParent.ToFullString();

				return new EvaluationResult
						   {
							   Comment = "Local time creation found.", 
							   Quality = CodeQuality.NeedsReview, 
							   QualityAttribute = QualityAttribute.Conformance, 
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}