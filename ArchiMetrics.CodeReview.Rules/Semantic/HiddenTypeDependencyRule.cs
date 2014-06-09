// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenTypeDependencyRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class HiddenTypeDependencyRule : SemanticEvaluationBase
	{
		private static readonly string[] SystemAssemblyPrefixes = new[] { "mscorlib", "System", "Microsoft", "PresentationFramework", "Windows" };

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

		protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
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
				.OfType<IFieldSymbol>()
				.Select(x => x.Type)
				.ToArray();
			var usedTypes = genericParameterTypes.Concat(fieldTypes)
				.WhereNotNull()
				.DistinctBy(x => x.ToDisplayString());
			var parameterTypes =
				methodDeclaration.ParameterList.Parameters.Select(x => semanticModel.GetSymbolInfo(x.Type).Symbol)
					.Concat(new[] { semanticModel.GetSymbolInfo(methodDeclaration.ReturnType).Symbol })
					.WhereNotNull()
					.DistinctBy(x => x.ToDisplayString())
					.ToArray();

			var parameterAssemblies = parameterTypes.Select(x => x.ContainingAssembly).ToArray();

			var locals = usedTypes.Except(parameterTypes);

			if (locals.Any(x => !x.ContainingAssembly.Equals(semanticModel.Compilation.Assembly) && !parameterAssemblies.Contains(x.ContainingAssembly) && !SystemAssemblyPrefixes.Any(y => x.ContainingAssembly.Name.StartsWith(y))))
			{
				return Task.FromResult(
					new EvaluationResult
					{
						Snippet = node.ToFullString()
					});
			}

			return Task.FromResult((EvaluationResult)null);
		}
	}
}