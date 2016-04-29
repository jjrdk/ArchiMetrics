// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskLocationDependencyRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DiskLocationDependencyRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Text.RegularExpressions;
	using Analysis.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class DiskLocationDependencyRule : CodeEvaluationBase
	{
		private static readonly Regex DiskLocationRegex = new Regex(@"\w:\\", RegexOptions.Compiled);

		public override string ID
		{
			get
			{
				return "AM0004";
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.SimpleAssignmentExpression;
			}
		}
		
		public override string Title
		{
			get
			{
				return "Disk Location Dependency";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Replace the dependency on a specific disk location with an abstraction.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsRefactoring;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Modifiability | QualityAttribute.Testability;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Project;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var assignExpression = (AssignmentExpressionSyntax)node;
			var right = assignExpression.Right as LiteralExpressionSyntax;
			if (right != null)
			{
				var assignmentToken = right.Token.ToFullString();
				if (DiskLocationRegex.IsMatch(assignmentToken))
				{
					return new EvaluationResult
							   {
								   Snippet = FindMethodParent(node).ToFullString()
							   };
				}
			}

			return null;
		}
	}
}
