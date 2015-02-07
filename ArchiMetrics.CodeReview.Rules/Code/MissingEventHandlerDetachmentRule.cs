// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MissingEventHandlerDetachmentRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MissingEventHandlerDetachmentRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class MissingEventHandlerDetachmentRule : CodeEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0027";
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.ClassDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Event Handler not Detached";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Unassign all event handlers.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.Broken;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Type;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declarationSyntax = (TypeDeclarationSyntax)node;
			var addAssignments = declarationSyntax
				.DescendantNodes()
				.Where(x => x.Kind() == SyntaxKind.AddAssignmentExpression)
				.Cast<AssignmentExpressionSyntax>()
				.AsArray();
			var subtractAssignments = declarationSyntax.DescendantNodes()
				.Where(x => x.Kind() == SyntaxKind.SubtractAssignmentExpression)
				.Cast<AssignmentExpressionSyntax>()
				.AsArray();

			var assignmentExpressionSyntaxes = addAssignments.DistinctBy(x => x.ToFullString()).AsArray();

			if (assignmentExpressionSyntaxes.Count() != subtractAssignments.DistinctBy(x => x.ToFullString()).Count())
			{
				var unmatched = assignmentExpressionSyntaxes.Where(x => !MatchingAssignmentExpressionExists(x, subtractAssignments));
				var snippet = string.Join(Environment.NewLine, unmatched.Select(x => x.ToFullString()));

				return new EvaluationResult
						   {
							   Snippet = snippet
						   };
			}

			return null;
		}

		private bool MatchingAssignmentExpressionExists(
			AssignmentExpressionSyntax addAssignment,
			IEnumerable<AssignmentExpressionSyntax> subtractAssignments)
		{
			var changedAssignment = SyntaxFactory.AssignmentExpression(
				SyntaxKind.SubtractAssignmentExpression,
				addAssignment.Left,
				addAssignment.Right);

			return subtractAssignments.Any(x => x.IsEquivalentTo(changedAssignment));
		}
	}
}