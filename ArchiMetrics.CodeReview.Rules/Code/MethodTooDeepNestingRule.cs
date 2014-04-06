namespace ArchiMetrics.CodeReview.Rules.Code
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class MethodTooDeepNestingRule : TooDeepNestingRuleBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.MethodDeclaration;
			}
		}

		protected override string NestingMember
		{
			get
			{
				return "Method";
			}
		}

		protected override BlockSyntax GetBody(SyntaxNode node)
		{
			var member = (MethodDeclarationSyntax)node;
			return member.Body;
		}
	}
}