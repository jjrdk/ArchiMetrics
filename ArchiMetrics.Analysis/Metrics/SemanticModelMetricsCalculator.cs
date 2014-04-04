// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticModelMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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
