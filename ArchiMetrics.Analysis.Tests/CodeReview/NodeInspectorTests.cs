// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeInspectorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeInspectorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.CodeReview
{
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Moq;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public sealed class NodeInspectorTests
	{
		private NodeInspectorTests()
		{
		}

		public class GivenANodeInspector
		{
			private NodeReviewer _reviewer;
			private Mock<ICodeEvaluation> _mockCodeEvaluation;

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
				var classDeclaration = Syntax.ClassDeclaration(
					Syntax.List<AttributeListSyntax>(),
					Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword)),
					Syntax.Identifier("MyClass"),
					Syntax.TypeParameterList(),
					Syntax.BaseList(),
					Syntax.List<TypeParameterConstraintClauseSyntax>(),
					Syntax.List<MemberDeclarationSyntax>());
				await _reviewer.Inspect("name", string.Empty, classDeclaration, null, null);

				_mockCodeEvaluation.Verify(x => x.Evaluate(classDeclaration));
			}
		}
	}
}
