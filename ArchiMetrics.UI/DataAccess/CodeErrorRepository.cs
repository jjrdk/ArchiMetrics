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
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class CodeErrorRepository : ICodeErrorRepository
	{
		private readonly IAvailableRules _availableRules;
		private readonly IAppContext _config;
		private readonly ConcurrentDictionary<string, Task<EvaluationResult[]>> _edgeItems;
		private readonly INodeInspector _inspector;
		private readonly IProvider<string, ISolution> _solutionProvider;

		public CodeErrorRepository(
			IAppContext config,
			IProvider<string, ISolution> solutionProvider,
			INodeInspector inspector,
			IAvailableRules availableRules)
		{
			_edgeItems = new ConcurrentDictionary<string, Task<EvaluationResult[]>>();
			_config = config;
			_solutionProvider = solutionProvider;
			_inspector = inspector;
			_availableRules = availableRules;
			_config.PropertyChanged += ConfigPropertyChanged;
			GetErrors(_config.Path);
		}

		~CodeErrorRepository()
		{
			Dispose(false);
		}

		public async Task<IEnumerable<EvaluationResult>> GetErrors(string source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			var results = await _edgeItems.GetOrAdd(source, LoadEvaluationResults);

			var availableRules = new HashSet<string>(_availableRules.Select(x => x.Title));
			return cancellationToken.IsCancellationRequested
					   ? Enumerable.Empty<EvaluationResult>()
					   : results.Where(x => availableRules.Contains(x.Title)).ToArray();
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
			var projects = solution.Projects.ToArray();
			var inspectionTasks = (from project in projects
								   where project.HasDocuments
								   let compilation = project.GetCompilationAsync()
								   from doc in project.Documents.ToArray()
								   let tree = doc.GetSyntaxTreeAsync()
								   select GetInspections(project.FilePath, tree, compilation, solution))
				.ToArray();
			if (inspectionTasks.Length == 0)
			{
				return new EvaluationResult[0];
			}

			var results = await Task.WhenAll(inspectionTasks);
			return results.SelectMany(x => x).Distinct(new ResultComparer()).ToArray();
		}

		private async Task<IEnumerable<EvaluationResult>> GetInspections(
			string filePath,
			Task<CommonSyntaxTree> treeTask,
			Task<CommonCompilation> compilationTask,
			ISolution solution)
		{
			var tree = await treeTask;
			var root = (await tree.GetRootAsync()) as SyntaxNode;
			if (root == null)
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			var compilation = await compilationTask;
			return await _inspector.Inspect(filePath, root, compilation.GetSemanticModel(tree), solution);
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Path")
			{
				return;
			}

			GetErrors(_config.Path);
		}
	}
}
