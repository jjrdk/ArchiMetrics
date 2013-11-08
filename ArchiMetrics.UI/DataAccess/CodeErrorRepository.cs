// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrorRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
		private readonly ConcurrentDictionary<string, Task<IEnumerable<EvaluationResult>>> _edgeItems;
		private readonly INodeInspector _inspector;
		private readonly IProvider<string, ISolution> _solutionProvider;

		public CodeErrorRepository(
			ISolutionEdgeItemsRepositoryConfig config, 
			IProvider<string, ISolution> solutionProvider, 
			INodeInspector inspector)
		{
			_edgeItems = new ConcurrentDictionary<string, Task<IEnumerable<EvaluationResult>>>();
			_config = config;
			_solutionProvider = solutionProvider;
			_inspector = inspector;
			_config.PropertyChanged += ConfigPropertyChanged;
			GetErrorsAsync();
		}

		~CodeErrorRepository()
		{
			Dispose(false);
		}

		public Task<IEnumerable<EvaluationResult>> GetErrorsAsync(string source, bool isTest)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				return Task.FromResult(Enumerable.Empty<EvaluationResult>());
			}

			return _edgeItems.GetOrAdd(
				source, 
				async path =>
				{
					var inspectionTasks = Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories).Where(p => !p.Contains("QuickStart"))
												   .Distinct()
												   .Select(_solutionProvider.Get)
												   .SelectMany(s => s.Projects.Distinct(ProjectComparer.Default).Select(_ => new { solution = s, project = _ }))
												   .SelectMany(p => p.project.Documents
													   .Distinct(DocumentComparer.Default)
													   .Select(d => new
														                {
															                solution = p.solution, 
																			project = p.project, 
																			document = d
														                })
												   .Select(d => new
													                {
														                projectPath = d.project.FilePath, 
																		syntaxTree = d.document.GetSyntaxTree().GetRoot() as SyntaxNode, 
																		solution = d.solution, 
																		semanticModel = d.document.GetSemanticModel()
													                }))
												   .Where(n => n.syntaxTree != null)
												   .Select(t => _inspector.Inspect(source, t.syntaxTree, t.semanticModel, t.solution))
												   .ToArray();
					if (inspectionTasks.Length == 0)
					{
						return new EvaluationResult[0];
					}

					return await Task.Factory
						.ContinueWhenAll(
							inspectionTasks,
							results => results.SelectMany(x => x.Result)
								.Distinct(new ResultComparer())
								.ToArray()
								.AsEnumerable());
				});
		}

		public IEnumerable<EvaluationResult> GetErrors()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<EvaluationResult> GetErrors(string obj0, bool isTest)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<EvaluationResult>> GetErrorsAsync()
		{
			return string.IsNullOrWhiteSpace(_config.Path)
								  ? Task.Factory.StartNew(() => new EvaluationResult[0].AsEnumerable())
								  : GetErrorsAsync(_config.Path, false);
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
				var tasks = _edgeItems.Values.ToArray();
				foreach (var task in tasks)
				{
					task.Dispose();
				}

				_edgeItems.Clear();

				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				_config.PropertyChanged -= ConfigPropertyChanged;
			}
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Path")
			{
				return;
			}

			GetErrorsAsync();
		}
	}
}
