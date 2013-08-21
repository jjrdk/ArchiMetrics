// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskLocationDependencyRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DiskLocationDependencyRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Code
{
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common;
	using Roslyn.Compilers.CSharp;

	internal class DiskLocationDependencyRule : CodeEvaluationBase
	{
		private static readonly Regex DiskLocationRegex = new Regex(@"\w:\\", RegexOptions.Compiled);

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.AssignExpression;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var assignExpression = (BinaryExpressionSyntax)node;
			var right = assignExpression.Right as LiteralExpressionSyntax;
			if (right != null)
			{
				var assignmentToken = right.Token.ToFullString();
				if (DiskLocationRegex.IsMatch(assignmentToken))
				{
					return new EvaluationResult
							   {
								   Comment = "FileSystem dependency detected",
								   Quality = CodeQuality.Broken,
								   QualityAttribute = QualityAttribute.Modifiability | QualityAttribute.Testability,
								   ImpactLevel = ImpactLevel.Project,
								   Snippet = FindMethodParent(node).ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
