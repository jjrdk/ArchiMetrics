// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoPublicFieldsRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoPublicFieldsRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class NoPublicFieldsRule : CodeEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0033";
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.FieldDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "No Public Field";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Encapsulate all public fields in properties, or internalize them.";
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
				return QualityAttribute.Modifiability;
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
			var syntax = (FieldDeclarationSyntax)node;
			if (syntax.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   };
			}

			return null;
		}
	}
}
