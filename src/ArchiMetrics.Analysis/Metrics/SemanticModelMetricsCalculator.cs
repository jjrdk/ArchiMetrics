// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticModelMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticModelMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using Microsoft.CodeAnalysis;

	public abstract class SemanticModelMetricsCalculator
	{
		private readonly SemanticModel _semanticModel;

		protected SemanticModelMetricsCalculator(SemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		protected SemanticModel Model
		{
			get { return _semanticModel; }
		}

		protected SyntaxNode Root
		{
			get { return _semanticModel.SyntaxTree.GetRoot(); }
		}
	}
}
