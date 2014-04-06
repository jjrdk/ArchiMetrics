namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal abstract class PropertyTooDeepNestingRule : TooDeepNestingRuleBase
	{
		protected override BlockSyntax GetBody(SyntaxNode node)
		{
			var property = (AccessorDeclarationSyntax)node;
			return property.Body;
		}
	}
}