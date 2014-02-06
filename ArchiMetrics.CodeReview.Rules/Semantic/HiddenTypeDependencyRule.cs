// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenTypeDependencyRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HiddenTypeDependencyRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Linq;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	internal class HiddenTypeDependencyRule : SemanticEvaluationBase
	{
		private static readonly string[] SystemAssemblyPrefixes = new[] { "System", "Microsoft", "PresentationFramework", "Windows" };

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MethodDeclaration;
			}
		}

		public override string Title
		{
			get
			{
				return "Hidden Type Dependency in " + EvaluatedKind.ToString().ToTitleCase();
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Refactor to pass dependencies explicitly.";
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
				return QualityAttribute.Maintainability | QualityAttribute.Modifiability | QualityAttribute.Testability;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Project;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node, ISemanticModel semanticModel, ISolution solution)
		{
			var methodDeclaration = (MethodDeclarationSyntax)node;
			if (methodDeclaration.Body == null)
			{
				return null;
			}

			var descendantNodes = methodDeclaration.Body.DescendantNodes().ToArray();
			var genericParameterTypes =
				descendantNodes.OfType<TypeArgumentListSyntax>()
					.SelectMany(x => x.Arguments.Select(y => semanticModel.GetSymbolInfo(y).Symbol));
			var symbolInfo = semanticModel.GetDeclaredSymbol(node);
			var containingType = symbolInfo.ContainingType;
			var fieldTypes = containingType.GetMembers()
				.OfType<FieldSymbol>().Select(x => x.Type)
				.ToArray();
			var usedTypes = genericParameterTypes.Concat(fieldTypes)
				.WhereNotNull()
				.DistinctBy(x => x.ToDisplayString());
			var parameterTypes =
				methodDeclaration.ParameterList.Parameters.Select(x => semanticModel.GetSymbolInfo(x.Type).Symbol)
					.WhereNotNull()
					.DistinctBy(x => x.ToDisplayString());

			var locals = usedTypes.Except(parameterTypes);

			if (locals.Any(x => !x.ContainingAssembly.Equals(semanticModel.Compilation.Assembly) && !SystemAssemblyPrefixes.Any(y => x.ContainingAssembly.Name.StartsWith(y))))
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