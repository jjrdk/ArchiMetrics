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
	using System.Collections.Concurrent;
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

		public async virtual Task<IEnumerable<EvaluationResult>> Inspect(
			string projectPath, 
			SyntaxNode node, 
			ISemanticModel semanticModel, 
			ISolution solution)
		{
			var inspector = new InnerInspector(_evaluations, semanticModel, solution);
			inspector.Visit(node);
			var inspectionResults = await inspector.GetResults();
			foreach (var result in inspectionResults.Where(x => x != null))
			{
				result.ProjectPath = projectPath;
			}

			var returnValue = inspectionResults.ToArray();
			inspector.Dispose();
			return returnValue.AsEnumerable();
		}

		private class InnerInspector : SyntaxWalker, IDisposable
		{
			private readonly IDictionary<SyntaxKind, IEvaluation[]> _evaluations;
			private readonly ConcurrentBag<EvaluationResult> _inspectionResults = new ConcurrentBag<EvaluationResult>();
			private readonly ConcurrentQueue<Task> _inspectionTasks = new ConcurrentQueue<Task>();
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

					var continuation = Task.WhenAll(codeResults, semmanticResults).ContinueWith(t =>
					{
						foreach (var source in t.Result.SelectMany(x => x).ToArray())
						{
							_inspectionResults.Add(source);
						}
					});

					_inspectionTasks.Enqueue(continuation);
				}

				base.Visit(node);
			}

			public override void VisitTrivia(SyntaxTrivia trivia)
			{
				if (_evaluations.ContainsKey(trivia.Kind))
				{
					var nodeEvaluations = _evaluations[trivia.Kind];
					var task = GetTriviaEvaluations(trivia, nodeEvaluations)
						.ContinueWith(
							t =>
								{
									foreach (var result in t.Result)
									{
										_inspectionResults.Add(result);
									}
								});

					_inspectionTasks.Enqueue(task);
				}

				base.VisitTrivia(trivia);
			}

			public async Task<EvaluationResult[]> GetResults()
			{
				await Task.WhenAll(_inspectionTasks);
				var results = _inspectionResults.Where(x => x != null).ToArray();
				while (_inspectionTasks.Count > 0)
				{
					Task t;
					_inspectionTasks.TryDequeue(out t);
					t.Dispose();
				}

				return results;
			}

			protected virtual void Dispose(bool isDisposing)
			{
				if (isDisposing)
				{
					// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
					while (_inspectionTasks.Count > 0)
					{
						Task t;
						_inspectionTasks.TryDequeue(out t);
						t.Dispose();
					}
				}
			}

			private Task<IEnumerable<EvaluationResult>> GetTriviaEvaluations(SyntaxTrivia trivia, IEnumerable<IEvaluation> nodeEvaluations)
			{
				return Task.Factory.StartNew(
					() =>
					{
						var results = nodeEvaluations
							.OfType<ITriviaEvaluation>()
							.Select(
								x =>
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
							.Where(x => x != null && x.Quality != CodeQuality.Good)
							.ToArray();
						return results.AsEnumerable();
					});
			}

			private Task<IEnumerable<EvaluationResult>> GetCodeEvaluations(SyntaxNode node, IEnumerable<IEvaluation> nodeEvaluations)
			{
				return Task.Factory.StartNew(() =>
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
							.Where(x => x != null && x.Quality != CodeQuality.Good)
							.ToArray()
							.AsEnumerable();
						return results;
					});
			}

			private Task<IEnumerable<EvaluationResult>> GetSemanticEvaluations(SyntaxNode node, IEnumerable<IEvaluation> nodeEvaluations, ISemanticModel model, ISolution solution)
			{
				return Task.Factory.StartNew(() =>
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
							.Where(x => x != null && x.Quality != CodeQuality.Good)
							.ToArray();
						return results;
					});
			}
		}
	}
}
