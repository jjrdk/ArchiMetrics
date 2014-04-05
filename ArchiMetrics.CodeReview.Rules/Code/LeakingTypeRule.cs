// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeakingTypeRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LeakingTypeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal abstract class LeakingTypeRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.PropertyDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Current Type Exposes " + TypeIdentifier;
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Remove public access to " + TypeIdentifier;
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
				return QualityAttribute.Modifiability | QualityAttribute.Reusability;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Member;
			}
		}

		protected abstract string TypeIdentifier { get; }

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var parentClass = FindClassParent(node);
			if (parentClass != null && parentClass.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				var propertyDeclaration = (PropertyDeclarationSyntax)node;
				if (propertyDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
				{
					var type = propertyDeclaration.Type as SimpleNameSyntax;
					if (type != null && type.Identifier.ValueText != null
						&& type.Identifier.ValueText.EndsWith(TypeIdentifier ?? string.Empty, StringComparison.InvariantCultureIgnoreCase))
					{
						return new EvaluationResult
								   {
									   Snippet = parentClass.ToFullString()
								   };
					}
				}
			}

			return null;
		}
	}
}
