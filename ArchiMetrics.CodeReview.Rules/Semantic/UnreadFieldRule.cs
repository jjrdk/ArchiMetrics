// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnreadFieldRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnreadFieldRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal class UnreadFieldRule : UnreadValueRule
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.FieldDeclaration; }
		}

		public override string Title
		{
			get { return "Field is never read"; }
		}

		public override string Suggestion
		{
			get { return "Remove unread field."; }
		}

		protected override IEnumerable<ISymbol> GetSymbols(SyntaxNode node, ISemanticModel semanticModel)
		{
			var declaration = (FieldDeclarationSyntax)node;

			var symbols = declaration.Declaration.Variables.Select(x => semanticModel.GetDeclaredSymbol(x)).ToArray();

			return symbols;
		}
	}
}