// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnusedFieldRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnusedFieldRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using ArchiMetrics.Common.CodeReview;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	internal class UnusedFieldRule : SemanticEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.FieldDeclaration; }
		}

		public override string Title
		{
			get { return "Field is never read"; }
		}

		public override string Suggestion
		{
			get { return "Remove unread field."; }
		}

		public override CodeQuality Quality
		{
			get { return CodeQuality.NeedsReview; }
		}

		public override QualityAttribute QualityAttribute
		{
			get { return QualityAttribute.CodeQuality | QualityAttribute.Maintainability; }
		}

		public override ImpactLevel ImpactLevel
		{
			get { return ImpactLevel.Type; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var declaration = (FieldDeclarationSyntax)node;

			var symbols = declaration.Declaration.Variables.Select(x => semanticModel.GetDeclaredSymbol(x)).ToArray();
			var callers = symbols
				.SelectMany(x => x.FindCallers(solution, CancellationToken.None));

			if (!callers.Any())
			{
				return new EvaluationResult
				{
					Snippet = declaration.ToFullString()
				};
			}

			return null;
		}
	}
}