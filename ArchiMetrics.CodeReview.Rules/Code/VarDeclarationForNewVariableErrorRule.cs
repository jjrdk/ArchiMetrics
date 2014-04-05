// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VarDeclarationForNewVariableErrorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VarDeclarationForNewVariableErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class VarDeclarationForNewVariableErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.VariableDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Var Keyword Used in Variable Declaration";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Consider using an explicit type for variable.";
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
				return QualityAttribute.Conformance;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Line;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var declaration = (VariableDeclarationSyntax)node;
			if (declaration.Type.IsVar && !declaration.Variables.All(x => x.Initializer.Value is ObjectCreationExpressionSyntax))
			{
				return new EvaluationResult
						   {
							   ErrorCount = declaration.Variables.Count, 
							   Snippet = declaration.ToFullString()
						   };
			}

			return null;
		}
	}
}
