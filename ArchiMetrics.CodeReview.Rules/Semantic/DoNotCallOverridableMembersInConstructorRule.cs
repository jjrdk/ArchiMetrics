// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoNotCallOverridableMembersInConstructorRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DoNotCallOverridableMembersInConstructorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class DoNotCallOverridableMembersInConstructorRule : SemanticEvaluationBase
	{
		public override string ID
		{
			get
			{
				return "CA2214";
			}
		}

		public override string Title
		{
			get
			{
				return "Do not call overridable methods in constructors";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Remove calls to virtual methods in constructor";
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
				return QualityAttribute.Modifiability | Common.CodeReview.QualityAttribute.Maintainability;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Type;
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.ConstructorDeclaration;
			}
		}

		protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			//// TODO: For this to be correct, we need flow analysis to determine if a given method
			//// is actually invoked inside the current constructor. A method may be assigned to a
			//// delegate which can be called inside or outside the constructor. A method may also
			//// be called from within a lambda which is called inside or outside the constructor.
			//// Currently, FxCop does not produce a warning if a virtual method is called indirectly
			//// through a delegate or through a lambda.

			var constructor = (ConstructorDeclarationSyntax)node;
			var constructorSymbol = semanticModel.GetDeclaredSymbol(constructor);
			var containingType = constructorSymbol.ContainingType;

			if (
				constructor.Body.DescendantNodes()
				.Where(x => x.CSharpKind() == SyntaxKind.InvocationExpression)
					.Any(x => CallVirtualMethod((InvocationExpressionSyntax)x, semanticModel, containingType)))
			{
				var result = new EvaluationResult
								 {
									 Snippet = node.ToFullString(),
									 LinesOfCodeAffected = GetLinesOfCode(node)
								 };
				return Task.FromResult(result);
			}

			return Task.FromResult<EvaluationResult>(null);
		}

		private static bool CallVirtualMethod(InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel, INamedTypeSymbol containingType)
		{
			var method = semanticModel.GetSymbolInfo(invocationExpression.Expression).Symbol as IMethodSymbol;
			return method != null
				&& (method.IsAbstract || method.IsVirtual)
				&& method.ContainingType == containingType;
		}
	}
}