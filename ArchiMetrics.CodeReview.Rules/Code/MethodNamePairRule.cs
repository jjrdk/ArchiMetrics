// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodNamePairRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
				return QualityAttribute.Conformance;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Type;
			}
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
				var pairMethodName = Regex.Replace(methodName, "^" + start, match);
				var parentClass = FindClassParent(method);
				if (parentClass == null)
				{
					return true;
				}

				return parentClass
					.ChildNodes()
					.OfType<MethodDeclarationSyntax>()
					.Any(m => m.Identifier.ValueText == pairMethodName);
			}

			return true;
		}
	}
}
