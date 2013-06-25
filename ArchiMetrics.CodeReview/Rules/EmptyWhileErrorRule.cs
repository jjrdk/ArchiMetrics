// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyWhileErrorRule.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EmptyWhileErrorRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Rules
{
	using System.Linq;
	using Common;
	using Roslyn.Compilers.CSharp;

	internal class EmptyWhileErrorRule : CodeEvaluationBase
	{
		public override SyntaxKind EvaluatedKind
		{
			get { return SyntaxKind.WhileStatement; }
		}

		protected override EvaluationResult EvaluateImpl(SyntaxNode node)
		{
			var whileStatement = (WhileStatementSyntax)node;

			var sleepLoopFound = whileStatement.DescendantNodes()
											   .OfType<BlockSyntax>()
											   .Any(s => !s.ChildNodes().Any());

			if (sleepLoopFound)
			{
				var snippet = FindMethodParent(node).ToFullString();

				return new EvaluationResult
					   {
						   Comment = "Empty while loop found in code.", 
						   Quality = CodeQuality.Incompetent, 
						   QualityAttribute = QualityAttribute.CodeQuality | QualityAttribute.Testability, 
						   Snippet = snippet
					   };
			}

			return null;
		}
	}
}