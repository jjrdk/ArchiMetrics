// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticModelMetricsCalculator.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticModelMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using Roslyn.Compilers.Common;

	public abstract class SemanticModelMetricsCalculator
	{
		private readonly ISemanticModel _semanticModel;

		// Methods
		protected SemanticModelMetricsCalculator(ISemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		// Properties
		protected ISemanticModel Model
		{
			get { return _semanticModel; }
		}

		protected CommonSyntaxNode Root
		{
			get { return _semanticModel.SyntaxTree.GetRoot(); }
		}
	}
}