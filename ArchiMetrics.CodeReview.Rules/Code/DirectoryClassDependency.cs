// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryClassDependency.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DirectoryClassDependency type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class DirectoryClassDependency : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MemberAccessExpression;
			}
		}

		public override string Title
		{
			get
			{
				return "Directory Class Dependency";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Consider breaking the direct dependency on the file system with an abstraction.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.Kind == SyntaxKind.IdentifierName
			    && ((IdentifierNameSyntax)memberAccess.Expression).Identifier.ValueText == "Directory")
			{
				var methodParent = FindMethodParent(node);
				var snippet = methodParent == null
					              ? FindClassParent(node).ToFullString()
					              : methodParent.ToFullString();

				return new EvaluationResult
					       {
						       Quality = CodeQuality.Broken, 
							   QualityAttribute = QualityAttribute.Modifiability | QualityAttribute.Testability, 
							   ImpactLevel = ImpactLevel.Type,
						       Snippet = snippet
					       };
			}

			return null;
		}
	}
}
