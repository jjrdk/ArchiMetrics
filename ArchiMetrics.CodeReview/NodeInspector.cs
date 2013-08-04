// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeInspector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeInspector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class NodeInspector : INodeInspector
	{
		private readonly Dictionary<SyntaxKind, IEvaluation[]> _evaluations;

		public NodeInspector(IEnumerable<IEvaluation> evaluations)
		{
			_evaluations = evaluations.GroupBy(x => x.EvaluatedKind).ToDictionary(x => x.Key, x => x.ToArray());
		}

		public virtual Task<IEnumerable<EvaluationResult>> Inspect(
			string projectPath, 
			SyntaxNode node, 
			ISemanticModel semanticModel, 
			ISolution solution)
		{
			return Task.Factory.StartNew(() =>
				{
					var inspector = new InnerInspector(_evaluations, semanticModel, solution);
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
			private readonly IDictionary<SyntaxKind, IEvaluation[]> _evaluations;

			private readonly List<EvaluationResult> _inspectionResults = new List<EvaluationResult>();

			private readonly ISemanticModel _model;
			private readonly ISolution _solution;

			public InnerInspector(IDictionary<SyntaxKind, IEvaluation[]> evaluations, ISemanticModel model, ISolution solution)
				: base(SyntaxWalkerDepth.Trivia)
			{
				_evaluations = evaluations;
				_model = model;
				_solution = solution;
			}

			~InnerInspector()
			{
				// Simply call Dispose(false).
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			public override void Visit(SyntaxNode node)
			{
				if (_evaluations.ContainsKey(node.Kind))
				{
					var nodeEvaluations = _evaluations[node.Kind];
					var codeResults = GetCodeEvaluations(node, nodeEvaluations);
					var semmanticResults = GetSemanticEvaluations(node, nodeEvaluations, _model, _solution);

					_inspectionResults.AddRange(codeResults);
					_inspectionResults.AddRange(semmanticResults);
				}

				base.Visit(node);
			}

			private IEnumerable<EvaluationResult> GetCodeEvaluations(SyntaxNode node, IEnumerable<IEvaluation> nodeEvaluations)
			{
				var results = nodeEvaluations
					.OfType<ICodeEvaluation>()
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
				return results;
			}

			private IEnumerable<EvaluationResult> GetSemanticEvaluations(SyntaxNode node, IEnumerable<IEvaluation> nodeEvaluations, ISemanticModel model, ISolution solution)
			{
				if (model == null || solution == null)
				{
					return Enumerable.Empty<EvaluationResult>();
				}

				var results = nodeEvaluations
					.OfType<ISemanticEvaluation>()
					.Select(x =>
						{
							try
							{
								return x.Evaluate(node, model, solution);
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
				return results;
			}

			public override void VisitTrivia(SyntaxTrivia trivia)
			{
				if (_evaluations.ContainsKey(trivia.Kind))
				{
					var nodeEvaluations = _evaluations[trivia.Kind];
					var results = nodeEvaluations
						.OfType<ITriviaEvaluation>()
						.Select(x =>
						{
							try
							{
								return x.Evaluate(trivia);
							}
							catch (Exception ex)
							{
								return new EvaluationResult
								{
									Comment = ex.Message, 
									ErrorCount = 1, 
									Snippet = trivia.ToFullString(), 
									Quality = CodeQuality.Broken
								};
							}
						})
						.Where(x => x != null && x.Quality != CodeQuality.Good);
					_inspectionResults.AddRange(results);
				}

				base.VisitTrivia(trivia);
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
