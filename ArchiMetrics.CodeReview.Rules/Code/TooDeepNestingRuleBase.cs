// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooDeepNestingRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TooDeepNestingRuleBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal abstract class TooDeepNestingRuleBase : CodeEvaluationBase
	{
		private readonly int _depth = 3;

		public TooDeepNestingRuleBase()
		{
		}

		public TooDeepNestingRuleBase(int maxDepth)
			: this()
		{
			_depth = maxDepth;
		}

		public override string Title
		{
			get
			{
				return "Too Deep Nesting";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Reduce nesting to make code more readable.";
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
				return QualityAttribute.Maintainability | QualityAttribute.Testability;
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
			var member = (MethodDeclarationSyntax)node;
			var body = member.Body;

			if (HasDeepNesting(body, 0))
			{
				return new EvaluationResult
					   {
						   Snippet = member.ToFullString()
					   };
			}

			return null;
		}

		private bool HasDeepNesting(BlockSyntax block, int level)
		{
			if (level >= _depth)
			{
				return true;
			}

			var result = GetBlocks(block).Aggregate(false, (a, b) => a || HasDeepNesting(b, level + 1));

			return result;
		}

		private IEnumerable<BlockSyntax> GetBlocks(SyntaxNode node)
		{
			var childBlocks = node.ChildNodes().Where(x => x.Kind == SyntaxKind.Block).Cast<BlockSyntax>();
			var others = node.ChildNodes()
				.Where(x => x.Kind != SyntaxKind.Block)
				.SelectMany(GetBlocks);

			return childBlocks.Concat(others);
		}
	}
}