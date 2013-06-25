// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicInterfaceImplementationWarningRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PublicInterfaceImplementationWarningRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Rules
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class PublicInterfaceImplementationWarningRule : CodeEvaluationBase
	{
		private static IEnumerable<Type> _appDomainTypes;
		
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.ClassDeclaration;
			}
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var classDeclaration = (ClassDeclarationSyntax)node;
			if (classDeclaration.BaseList != null && (classDeclaration.BaseList.Types.Any(SyntaxKind.IdentifierName) || classDeclaration.BaseList.Types.Any(SyntaxKind.GenericName)))
			{
				var s = classDeclaration.BaseList.Types.First(x => x.Kind == SyntaxKind.IdentifierName || x.Kind == SyntaxKind.GenericName);
				if (((SimpleNameSyntax)s).Identifier.ValueText.StartsWith("I")
					&& classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword))
				{
					var interfaceName = ((SimpleNameSyntax)s).Identifier.ValueText;
					if (!IsKnownInterface(interfaceName))
					{
						var snippet = classDeclaration.ToFullString();

						return new EvaluationResult
								   {
									   Comment = "Public class as known interface implementation.", 
									   Quality = CodeQuality.NeedsReview, 
									   QualityAttribute = QualityAttribute.Modifiability, 
									   Snippet = snippet
								   };
					}
				}
			}

			return null;
		}

		private bool IsKnownInterface(string interfaceName)
		{
			try
			{
				var types = _appDomainTypes ?? (_appDomainTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()));
				return types
								.Any(
									t =>
									string.Equals(t.Name, interfaceName, StringComparison.InvariantCultureIgnoreCase)
									|| string.Equals(t.FullName, interfaceName));
			}
			catch
			{
				return false;
			}
		}
	}
}
