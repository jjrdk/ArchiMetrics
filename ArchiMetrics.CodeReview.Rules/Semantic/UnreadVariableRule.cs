// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadVariableRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadVariableRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class UnreadVariableRule : UnreadValueRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.VariableDeclaration; }
		}

		public override string Title
		{
			get { return "Variable is never read"; }
		}

		public override string Suggestion
		{
			get { return "Remove unread variable."; }
		}

		protected override IEnumerable<ISymbol> GetSymbols(SyntaxNode node, SemanticModel semanticModel)
		{
			var declaration = (VariableDeclarationSyntax)node;

			var symbols = declaration.Variables.Select(x => semanticModel.GetDeclaredSymbol(x)).ToArray();

			return symbols;
		}
	}
}