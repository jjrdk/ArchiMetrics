// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionShouldBeVisibleRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExceptionShouldBeVisibleRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.CodeReview.Rules.Semantic;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	////internal class ExceptionShouldBeVisibleRule : SemanticEvaluationBase
	////{
	////	private static readonly string[] DisallowedExceptions = { "Exception", "SystemException", "ApplicationException" };

	////	public override string ID
	////	{
	////		get
	////		{
	////			return string.Empty;
	////		}
	////	}

	////	public override string Suggestion
	////	{
	////		get
	////		{
	////			return "This exception is not public and its base class does not provide enough information to be useful.";
	////		}
	////	}

	////	public override CodeQuality Quality
	////	{
	////		get
	////		{
	////			return CodeQuality.NeedsReview;
	////		}
	////	}

	////	public override QualityAttribute QualityAttribute
	////	{
	////		get
	////		{
	////			return QualityAttribute.Testability;
	////		}
	////	}

	////	public override ImpactLevel ImpactLevel
	////	{
	////		get
	////		{
	////			return ImpactLevel.Project;
	////		}
	////	}

	////	public override SyntaxKind EvaluatedKind
	////	{
	////		get
	////		{
	////			return SyntaxKind.ClassDeclaration;
	////		}
	////	}

	////	public override string Title
	////	{
	////		get
	////		{
	////			return "Exception should be visible.";
	////		}
	////	}

	////	protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
	////	{
	////		var declaration = (ClassDeclarationSyntax)node;
	////		var baseTypes = declaration.BaseList;
	////		if (baseTypes == null
	////			|| !baseTypes.Types.Any()
	////			|| baseTypes.Types.All(x =>
	////				{
	////					// rule apply only to type that inherits from the base exceptions
	////					var symbol = semanticModel.GetDeclaredSymbol(x);
	////					return symbol == null
	////						   || symbol.ContainingNamespace.Name != "System"
	////						   || !DisallowedExceptions.Contains(symbol.Name)
	////						   || symbol.IsAbstract
	////						   || symbol.DeclaredAccessibility == Accessibility.Public;
	////				}))
	////		{
	////			Task.FromResult<EvaluationResult>(null);
	////		}

	////		return Task.FromResult(new EvaluationResult
	////								   {
	////									   Snippet = declaration.ToFullString()
	////								   });
	////	}
	////}
}