// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmediateTaskWaitRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ImmediateTaskWaitRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using System.Linq;
	using Analysis.Common;
	using Analysis.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class ImmediateTaskWaitRule : CodeEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0017";
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.SimpleMemberAccessExpression;
			}
		}

		public override string Title
		{
			get
			{
				return "Immediate Task Wait.";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Immediately awaiting a Task has same effect as executing code synchonously.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsCleanup;
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
				return ImpactLevel.Member;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var memberAccess = (MemberAccessExpressionSyntax)node;
			if (memberAccess.Expression.IsKind(SyntaxKind.IdentifierName)
				&& memberAccess.Name.Identifier.ValueText == "Wait")
			{
				var invokedVariable = memberAccess.Expression as IdentifierNameSyntax;
				if (invokedVariable != null)
				{
					var variableName = invokedVariable.Identifier.ValueText;
					var methodParent = FindMethodParent(node);
					var variableAssignment = methodParent == null ? null : FindVariableAssignment(methodParent, variableName);
					if (variableAssignment != null)
					{
						var childNodes = memberAccess.Parent.Parent.Parent.ChildNodes().Select(n => n.WithLeadingTrivia().WithTrailingTrivia().ToString()).AsArray();
						var assignmentIndex = Array.IndexOf(childNodes, variableAssignment.Parent.WithLeadingTrivia().WithTrailingTrivia() + ";");
						var invocationIndex = Array.IndexOf(childNodes, memberAccess.Parent.WithLeadingTrivia().WithTrailingTrivia() + ";");
						if (invocationIndex == assignmentIndex + 1)
						{
							var snippet = methodParent.ToFullString();

							return new EvaluationResult
									   {
										   Snippet = snippet
									   };
						}
					}
				}
			}

			return null;
		}

		private SyntaxNode FindVariableAssignment(SyntaxNode node, string variableName)
		{
			return node.DescendantNodes()
					   .Where(n => n.IsKind(SyntaxKind.SimpleAssignmentExpression))
					   .OfType<AssignmentExpressionSyntax>()
					   .Select(x => x.Left as IdentifierNameSyntax)
					   .Where(x => x != null).FirstOrDefault(x => x.Identifier.ValueText == variableName);
		}
	}
}
