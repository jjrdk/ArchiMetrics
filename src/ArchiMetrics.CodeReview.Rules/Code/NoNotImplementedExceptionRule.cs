// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoNotImplementedExceptionRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NoNotImplementedExceptionRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
    using Analysis.Common.CodeReview;
    using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class NoNotImplementedExceptionRule : CodeEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "AM0030";
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.ThrowStatement; }
		}

		public override string Title
		{
			get { return "NotImplementedException Thrown"; }
		}

		public override string Suggestion
		{
			get { return "Add method implementation."; }
		}

		public override CodeQuality Quality
		{
			get { return CodeQuality.Broken; }
		}

		public override QualityAttribute QualityAttribute
		{
			get { return QualityAttribute.CodeQuality; }
		}

		public override ImpactLevel ImpactLevel
		{
			get { return ImpactLevel.Member; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var statement = (ThrowStatementSyntax)node;
			var exceptionCreation = statement.Expression as ObjectCreationExpressionSyntax;
			if (exceptionCreation != null)
			{
				var exceptionType = exceptionCreation.Type as IdentifierNameSyntax;
				if (exceptionType != null && exceptionType.Identifier.ValueText.EndsWith("NotImplementedException"))
				{
					return new EvaluationResult
					{
						Snippet = node.ToFullString()
					};
				}
			}

			return null;
		}
	}
}