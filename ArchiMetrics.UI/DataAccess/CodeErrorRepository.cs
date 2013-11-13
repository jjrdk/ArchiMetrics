// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrorRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeErrorRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class CodeErrorRepository : ICodeErrorRepository
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly ConcurrentDictionary<string, EvaluationResult[]> _edgeItems;
		private readonly INodeInspector _inspector;
		private readonly IProvider<string, ISolution> _solutionProvider;

		public CodeErrorRepository(
			ISolutionEdgeItemsRepositoryConfig config,
			IProvider<string, ISolution> solutionProvider,
			INodeInspector inspector)
		{
			_edgeItems = new ConcurrentDictionary<string, EvaluationResult[]>();
			_config = config;
			_solutionProvider = solutionProvider;
			_inspector = inspector;
			_config.PropertyChanged += ConfigPropertyChanged;
			GetErrors();
		}

		~CodeErrorRepository()
		{
			Dispose(false);
		}

		public Task<IEnumerable<EvaluationResult>> GetErrors(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				return Task.FromResult(Enumerable.Empty<EvaluationResult>());
			}

			return Task.Factory.StartNew(
				() =>
				{
					var cachedEdges = _edgeItems.GetOrAdd(
						source,
						path =>
						{
							var loadTask = LoadEvaluationResults(path);
							return loadTask.Result;
						});

					return cachedEdges.AsEnumerable();
				});
		}

		public Task<IEnumerable<EvaluationResult>> GetErrors()
		{
			return string.IsNullOrWhiteSpace(_config.Path)
								  ? Task.Factory.StartNew(() => new EvaluationResult[0].AsEnumerable())
								  : GetErrors(_config.Path);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_edgeItems.Clear();
				_config.PropertyChanged -= ConfigPropertyChanged;
			}
		}

		private async Task<EvaluationResult[]> LoadEvaluationResults(string path)
		{
			var solution = _solutionProvider.Get(path);
			var inspectionTasks = solution.Projects
				.Select(
					_ => new
						 {
							 solution = solution,
							 project = _
						 })
				.SelectMany(
					p => p.project.Documents
							 .Distinct(DocumentComparer.Default)
							 .Select(
								 d => new
									  {
										  solution = p.solution,
										  project = p.project,
										  document = d
									  })
							 .Select(
								 d => new
									  {
										  projectPath = d.project.FilePath,
										  syntaxTree = d.document.GetSyntaxTree()
														   .GetRoot() as SyntaxNode,
										  solution = d.solution,
										  semanticModel = d.document.GetSemanticModel()
									  }))
				.Where(n => n.syntaxTree != null)
				.AsParallel()
				.Select(t => _inspector.Inspect(t.projectPath, t.syntaxTree, t.semanticModel, t.solution))
				.ToArray();
			if (inspectionTasks.Length == 0)
			{
				return new EvaluationResult[0];
			}

			var results = await Task.WhenAll(inspectionTasks);
			return results.SelectMany(x => x)
				.Distinct(new ResultComparer())
				.ToArray();
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Path")
			{
				return;
			}

			GetErrors();
		}
	}
}
