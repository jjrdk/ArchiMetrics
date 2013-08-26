// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LackOfCohesionOfMethodsRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	internal class LackOfCohesionOfMethodsRule : SemanticEvaluationBase
	{
		private static readonly CommonSymbolKind[] MemberKinds =
		{
			CommonSymbolKind.Event, 
			CommonSymbolKind.Method, 
			CommonSymbolKind.Property
		};

		private int _threshold = 10;

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

		protected override EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			var symbol = (ITypeSymbol)semanticModel.GetDeclaredSymbol(classDeclaration);
			var members = symbol.GetMembers();

			var memberCount = members.Where(x => MemberKinds.Contains(x.Kind)).Count();
			if (memberCount < _threshold)
			{
				return null;
			}

			var fields = members.Where(x => x.Kind == CommonSymbolKind.Field).ToArray();
			var fieldCount = fields.Length;

			if (fieldCount < _threshold)
			{
				return null;
			}

			var sumFieldUsage = (double)fields.Sum(f => f.FindReferences(solution)
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
					   ImpactLevel = ImpactLevel.Type, 
					   LinesOfCodeAffected = GetLinesOfCode(snippet), 
					   Quality = CodeQuality.NeedsRefactoring, 
					   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Maintainability, 
					   Snippet = snippet
				   };
		}

		public void SetThreshold(int threshold)
		{
			_threshold = threshold;
		}
	}
}