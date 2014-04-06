namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

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

		protected override IEnumerable<ISymbol> GetSymbols(SyntaxNode node, SemanticModel semanticModel)
		{
			var declaration = (FieldDeclarationSyntax)node;

			var symbols = declaration.Declaration.Variables.Select(x => semanticModel.GetDeclaredSymbol(x)).ToArray();

			return symbols;
		}
	}
}