// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyTestRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyTestRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using Analysis.Common;
	using Analysis.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class EmptyTestRule : CodeEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0009";
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		public override string Title
		{
			get
			{
				return "No Assertion in Test";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Add an assertion to the test.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsReview;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Testability | QualityAttribute.CodeQuality;
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
			var methodParent = (MethodDeclarationSyntax)node;

			if (methodParent != null
				&& methodParent.AttributeLists.Any(
					l => l.Attributes.Any(a => a.Name is SimpleNameSyntax
											   && ((SimpleNameSyntax)a.Name).Identifier.ValueText.IsKnownTestAttribute())))
			{
				if (methodParent.Body == null
					|| !methodParent.Body.ChildNodes().Any())
				{
					return new EvaluationResult
						   {
							   Snippet = (FindClassParent(node) ?? node).ToFullString()
						   };
				}
			}

			return null;
		}
	}
}