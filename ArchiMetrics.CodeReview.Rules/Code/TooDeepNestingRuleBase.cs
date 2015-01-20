// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooDeepNestingRuleBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal abstract class TooDeepNestingRuleBase : CodeEvaluationBase
	{
		private readonly int _depth;

		protected TooDeepNestingRuleBase()
			: this(3)
		{
		}

		protected TooDeepNestingRuleBase(int maxDepth)
		{
			_depth = maxDepth;
		}

		public override string ID
		{
			get
			{
				return "AM0045";
			}
		}

		public override string Title
		{
			get
			{
				return "Too Deep " + NestingMember + " Nesting";
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

		protected abstract string NestingMember { get; }

		protected abstract BlockSyntax GetBody(SyntaxNode node);

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var body = GetBody(node);
			if (body != null && HasDeepNesting(body, 0))
			{
				return new EvaluationResult
					   {
						   Snippet = node.ToFullString()
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
			var childBlocks = node.ChildNodes().Where(x => x.IsKind(SyntaxKind.Block)).Cast<BlockSyntax>();
			var others = node.ChildNodes()
				.Where(x => !x.IsKind(SyntaxKind.Block))
				.SelectMany(GetBlocks);

			return childBlocks.Concat(others);
		}
	}
}