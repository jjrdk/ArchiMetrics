// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodNamePairRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodNamePairRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Code
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common.CodeReview;
	using Roslyn.Compilers.CSharp;

	internal abstract class MethodNamePairRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.MethodDeclaration; }
		}

		protected abstract string BeginToken { get; }

		protected abstract string PairToken { get; }

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var method = (MethodDeclarationSyntax)node;
			if (!HasMatchingMethod(BeginToken, PairToken, method))
			{
				return new EvaluationResult
						   {
							   Comment = "Pair method missing", 
							   ErrorCount = 1, 
							   ImpactLevel = ImpactLevel.Type, 
							   Quality = CodeQuality.NeedsRefactoring, 
							   QualityAttribute = QualityAttribute.Conformance, 
							   Snippet = method.ToFullString()
						   };
			}

			return null;
		}

		private bool HasMatchingMethod(string start, string match, MethodDeclarationSyntax method)
		{
			var methodName = method.Identifier.ValueText;
			if (methodName.StartsWith(start, StringComparison.InvariantCultureIgnoreCase))
			{
				var parentClass = FindClassParent(method);
				var pairMethodName = Regex.Replace(methodName, "^" + start, match);
				return parentClass
					.ChildNodes()
					.OfType<MethodDeclarationSyntax>()
					.Any(m => m.Identifier.ValueText == pairMethodName);
			}

			return true;
		}
	}
}
