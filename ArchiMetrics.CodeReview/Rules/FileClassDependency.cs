// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileClassDependency.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FileClassDependency type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class FileClassDependency : CodeEvaluationBase
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
				&& ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "File")
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent == null
								  ? FindClassParent(node).ToFullString()
								  : methodParent.ToFullString();

				return new EvaluationResult
						   {
							   Comment = "File dependency found.", 
							   Quality = CodeQuality.Broken, 
							   QualityAttribute = QualityAttribute.Modifiability | QualityAttribute.Testability, 
							   Snippet = snippet
						   };
			}

			return null;
		}
	}
}