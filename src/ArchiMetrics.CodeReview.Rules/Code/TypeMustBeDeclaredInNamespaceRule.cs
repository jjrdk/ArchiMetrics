// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMustBeDeclaredInNamespaceRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMustBeDeclaredInNamespaceRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
    using Analysis.Common.CodeReview;
    using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;

	internal class TypeMustBeDeclaredInNamespaceRule : CodeEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0048";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Move type declaration inside namespace.";
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
				return ImpactLevel.Type;
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
				return "Declare Types Inside Namespace.";
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var ns = FindNamespaceParent(node);
			if (ns == null)
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