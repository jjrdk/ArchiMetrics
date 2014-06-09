// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LackOfCohesionOfMethodsRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LackOfCohesionOfMethodsRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.FindSymbols;

	internal class LackOfCohesionOfMethodsRule : SemanticEvaluationBase
	{
		private static readonly SymbolKind[] MemberKinds =
		{
			SymbolKind.Event, 
			SymbolKind.Method, 
			SymbolKind.Property
		};

		private int _threshold = 6;

		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.ClassDeclaration; }
		}

		public override string Title
		{
			get
			{
				return "Lack of Cohesion of Methods";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor class into separate classes with single responsibility.";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.NeedsRefactoring;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality | QualityAttribute.Maintainability;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Type;
			}
		}

		public void SetThreshold(int threshold)
		{
			_threshold = threshold;
		}

		protected override async Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			var symbol = (ITypeSymbol)ModelExtensions.GetDeclaredSymbol(semanticModel, classDeclaration);
			var members = symbol.GetMembers();

			var memberCount = members.Count(x => MemberKinds.Contains(x.Kind));
			if (memberCount < _threshold)
			{
				return null;
			}

			var fields = members.Where(x => x.Kind == SymbolKind.Field).ToArray();
			var fieldCount = fields.Length;

			if (fieldCount < _threshold)
			{
				return null;
			}

			var references = await Task.WhenAll(fields.Select(solution.FindReferences)).ConfigureAwait(false);
			var sumFieldUsage = (double)references.Sum(
				r => r
						 .SelectMany(x => x.Locations)
						 .Count());

			var lcomhs = (memberCount - (sumFieldUsage / fieldCount)) / (memberCount - 1);
			if (lcomhs < 1)
			{
				return null;
			}

			var snippet = node.ToFullString();
			return new EvaluationResult
				   {
					   Snippet = snippet
				   };
		}
	}
}