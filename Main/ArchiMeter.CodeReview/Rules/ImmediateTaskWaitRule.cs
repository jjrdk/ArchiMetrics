// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmediateTaskWaitRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ImmediateTaskWaitRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using System;
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class ImmediateTaskWaitRule : EvaluationBase
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
						var childNodes = memberAccess.Parent.Parent.Parent.ChildNodes().Select(n => n.WithLeadingTrivia().WithTrailingTrivia().ToString()).ToArray();
						var assignmentIndex = Array.IndexOf(childNodes, variableAssignment.Parent.WithLeadingTrivia().WithTrailingTrivia() + ";");
						var invocationIndex = Array.IndexOf(childNodes, memberAccess.Parent.WithLeadingTrivia().WithTrailingTrivia() + ";");
						if (invocationIndex == assignmentIndex + 1)
						{
							var snippet = methodParent.ToFullString();

							return new EvaluationResult
									   {
										   Comment = "Immediately waiting on newly created asynchronous Task.", 
										   Quality = CodeQuality.Incompetent, 
										   QualityAttribute = QualityAttribute.CodeQuality, 
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
					   .Where(n => n.Kind == SyntaxKind.AssignExpression)
					   .OfType<BinaryExpressionSyntax>()
					   .Select(x => x.Left as IdentifierNameSyntax)
					   .Where(x => x != null).FirstOrDefault(x => x.Identifier.ValueText == variableName);
		}
	}
}