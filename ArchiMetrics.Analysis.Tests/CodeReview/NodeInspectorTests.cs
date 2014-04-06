namespace ArchiMetrics.Analysis.Tests.CodeReview
{
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Moq;
	using NUnit.Framework;

	public sealed class NodeInspectorTests
	{
		private NodeInspectorTests()
		{
		}

		public class GivenANodeInspector
		{
			private Mock<ICodeEvaluation> _mockCodeEvaluation;
			private NodeReviewer _reviewer;

			[SetUp]
			public void Setup()
			{
				_mockCodeEvaluation = new Mock<ICodeEvaluation>();
				_mockCodeEvaluation.SetupGet(x => x.EvaluatedKind).Returns(SyntaxKind.ClassDeclaration);
				_mockCodeEvaluation.Setup(x => x.Evaluate(It.IsAny<SyntaxNode>())).Returns((EvaluationResult)null);
				_reviewer = new NodeReviewer(new[] { _mockCodeEvaluation.Object });
			}

			[Test]
			public async Task WhenEvaluatingCodeThenCallsCodeEvaluation()
			{
				var classDeclaration = SyntaxFactory.ClassDeclaration(
					SyntaxFactory.List<AttributeListSyntax>(),
					SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
					SyntaxFactory.Identifier("MyClass"),
					SyntaxFactory.TypeParameterList(),
					SyntaxFactory.ParameterList(),
					SyntaxFactory.BaseList(),
					SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
					SyntaxFactory.List<MemberDeclarationSyntax>());
				await _reviewer.Inspect("name", string.Empty, classDeclaration, null, null);

				_mockCodeEvaluation.Verify(x => x.Evaluate(classDeclaration));
			}
		}
	}
}
