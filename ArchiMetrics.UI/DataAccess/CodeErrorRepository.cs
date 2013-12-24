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
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class CodeErrorRepository : ICodeErrorRepository
	{
		private readonly IAvailableRules _availableRules;
		private readonly IAppContext _config;
		private readonly ConcurrentDictionary<string, Lazy<EvaluationResult[]>> _edgeItems;
		private readonly INodeInspector _inspector;
		private readonly IProvider<string, ISolution> _solutionProvider;

		public CodeErrorRepository(
			IAppContext config, 
			IProvider<string, ISolution> solutionProvider, 
			INodeInspector inspector, 
			IAvailableRules availableRules)
		{
			_edgeItems = new ConcurrentDictionary<string, Lazy<EvaluationResult[]>>();
			_config = config;
			_solutionProvider = solutionProvider;
			_inspector = inspector;
			_availableRules = availableRules;
			_config.PropertyChanged += ConfigPropertyChanged;
			GetErrors();
		}

		~CodeErrorRepository()
		{
			Dispose(false);
		}

		public Task<IEnumerable<EvaluationResult>> GetErrors(string source, CancellationToken cancellationToken = default(CancellationToken))
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
							var loadTask = new Lazy<EvaluationResult[]>(() => LoadEvaluationResults(path), LazyThreadSafetyMode.ExecutionAndPublication);
							return loadTask;
						});

					var availableRules = new HashSet<string>(_availableRules.Select(x => x.Title));
					return cancellationToken.IsCancellationRequested
						? Enumerable.Empty<EvaluationResult>()
						: cachedEdges.Value
							.Where(x => availableRules.Contains(x.Title))
							.ToArray()
							.AsEnumerable();
				}, 
				cancellationToken);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Task<IEnumerable<EvaluationResult>> GetErrors(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _config.IncludeCodeReview && !string.IsNullOrWhiteSpace(_config.Path)
					   ? GetErrors(_config.Path, cancellationToken)
					   : Task.Factory.StartNew(() => new EvaluationResult[0].AsEnumerable(), cancellationToken);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_edgeItems.Clear();
				_config.PropertyChanged -= ConfigPropertyChanged;
			}
		}

		private EvaluationResult[] LoadEvaluationResults(string path)
		{
			var solution = _solutionProvider.Get(path);

			var inspectionTasks = (from project in solution.Projects
								   where project.HasDocuments
								   let compilation = project.GetCompilation()
								   from doc in project.Documents
								   let tree = doc.GetSyntaxTree()
								   let root = tree.GetRoot() as SyntaxNode
								   where root != null
								   select _inspector.Inspect(project.FilePath, root, compilation.GetSemanticModel(tree), solution))
				.ToArray();
			if (inspectionTasks.Length == 0)
			{
				return new EvaluationResult[0];
			}

			Task.WaitAll(inspectionTasks);

			return inspectionTasks.SelectMany(x => x.Result)
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
