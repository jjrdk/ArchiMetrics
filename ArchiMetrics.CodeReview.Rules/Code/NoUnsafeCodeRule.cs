// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoUnsafeCodeRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoUnsafeCodeRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal class NoUnsafeCodeRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.UnsafeStatement;
			}
		}

		public override string Title
		{
			get
			{
				return "Unsafe Statement Detected";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Avoid unsafe code.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsReEngineering;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.Conformance | QualityAttribute.Security;
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
			var snippet = node.ToFullString();
			return new EvaluationResult
				   {
					   LinesOfCodeAffected = GetLinesOfCode(snippet),
					   Snippet = snippet
				   };
		}
	}
}