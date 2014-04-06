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