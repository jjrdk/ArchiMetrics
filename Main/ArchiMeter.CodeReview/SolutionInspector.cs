// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionInspector.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionInspector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Roslyn.Compilers.CSharp;

	public class SolutionInspector : INodeInspector
	{
		private readonly Dictionary<SyntaxKind, ICodeEvaluation[]> _evaluations;

		public SolutionInspector(IEnumerable<ICodeEvaluation> evaluations)
		{
			_evaluations = evaluations.GroupBy(x => x.EvaluatedKind).ToDictionary(x => x.Key, x => x.ToArray());
		}

		public virtual Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, SyntaxNode node)
		{
			return Task.Factory.StartNew(() =>
				{
					var inspector = new InnerInspector(_evaluations);
					inspector.Visit(node);
					var inspectionResults = inspector.GetResults();
					foreach (var result in inspectionResults)
					{
						result.ProjectPath = projectPath;
					}

					var returnValue = inspectionResults.ToArray();
					inspector.Dispose();
					return returnValue.AsEnumerable();
				});
		}

		private class InnerInspector : SyntaxWalker, IDisposable
		{
			private readonly IDictionary<SyntaxKind, ICodeEvaluation[]> _evaluations;
			private readonly List<EvaluationResult> _inspectionResults = new List<EvaluationResult>();

			public InnerInspector(IDictionary<SyntaxKind, ICodeEvaluation[]> evaluations)
			{
				_evaluations = evaluations;
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			~InnerInspector()
			{
				// Simply call Dispose(false).
				Dispose(false);
			}

			public override void Visit(SyntaxNode node)
			{
				if (_evaluations.ContainsKey(node.Kind))
				{
					var nodeEvaluations = _evaluations[node.Kind];
					var results = nodeEvaluations
						.Select(x =>
							{
								try
								{
									return x.Evaluate(node);
								}
								catch (Exception ex)
								{
									return new EvaluationResult
											   {
												   Comment = ex.Message, 
												   ErrorCount = 1, 
												   Snippet = node.ToFullString(), 
												   Quality = CodeQuality.Broken
											   };
								}
							})
						.Where(x => x != null && x.Quality != CodeQuality.Good);
					_inspectionResults.AddRange(results);
				}

				base.Visit(node);
			}

			public EvaluationResult[] GetResults()
			{
				return _inspectionResults.ToArray();
			}

			protected virtual void Dispose(bool isDisposing)
			{
				if (isDisposing)
				{
					// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
					_inspectionResults.Clear();
				}
			}
		}
	}
}
