// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticModelMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using Roslyn.Compilers.Common;

	public abstract class SemanticModelMetricsCalculator
	{
		private readonly ISemanticModel _semanticModel;

		protected SemanticModelMetricsCalculator(ISemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

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
