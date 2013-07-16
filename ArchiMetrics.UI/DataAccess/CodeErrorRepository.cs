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
			this._edgeItems = new ConcurrentDictionary<string, Task<IEnumerable<EvaluationResult>>>();
			this._config = config;
			this._solutionProvider = solutionProvider;
			this._inspector = inspector;
			this._config.PropertyChanged += this.ConfigPropertyChanged;
			this.GetErrorsAsync();
		}

		~CodeErrorRepository()
		{
			this.Dispose(false);
		}

		public Task<IEnumerable<EvaluationResult>> GetErrorsAsync(string source, bool isTest)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				return Task.FromResult(Enumerable.Empty<EvaluationResult>());
			}

			return this._edgeItems.GetOrAdd(
				source,
				async path =>
				{
					var inspectionTasks = Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories).Where(p => !p.Contains("QuickStart"))
												   .Distinct()
												   .Select(this._solutionProvider.Get)
												   .SelectMany(s => s.Projects)
												   .Distinct(ProjectComparer.Default)
												   .SelectMany(p => p.Documents)
												   .Distinct(DocumentComparer.Default)
												   .Select(d => new Tuple<string, SyntaxNode>(d.Project.FilePath, d.GetSyntaxTree().GetRoot() as SyntaxNode))
												   .Where(n => n.Item2 != null)
												   .Select(t => this._inspector.Inspect(t.Item1, t.Item2));

					return await Task.Factory
									 .ContinueWhenAll(
										 inspectionTasks.ToArray(),
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
			return string.IsNullOrWhiteSpace(this._config.Path)
								  ? Task.Factory.StartNew(() => new EvaluationResult[0].AsEnumerable())
								  : this.GetErrorsAsync(this._config.Path, false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				var tasks = this._edgeItems.Values.ToArray();
				foreach (var task in tasks)
				{
					task.Dispose();
				}

				this._edgeItems.Clear();

				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				this._config.PropertyChanged -= this.ConfigPropertyChanged;
			}
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "Path")
			{
				return;
			}

			this.GetErrorsAsync();
		}
	}
}
