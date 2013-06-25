// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationResultLoader.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationResultLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.DataLoader
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Common.Documents;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class EvaluationResultLoader : IDataLoader
	{
		private readonly INodeInspector _inspector;
		private readonly IProvider<string, IProject> _projectProvider;
		private readonly IFactory<IDataSession<EvaluationResultDocument>> _sessionProvider;

		public EvaluationResultLoader(
			INodeInspector nodeInspector, 
			IFactory<IDataSession<EvaluationResultDocument>> sessionProvider,
			IProvider<string, IProject> projectProvider)
		{
			_projectProvider = projectProvider;
			_inspector = nodeInspector;
			_sessionProvider = sessionProvider;
		}

		public async Task Load(ProjectSettings settings)
		{
			Console.WriteLine("Loading Evaluation Results for " + settings.Name);

			var projects = (from root in settings.Roots
							from project in _projectProvider.GetAll(root.Source)
							from document in project.Documents
							where string.Equals(Path.GetExtension(document.FilePath), ".cs", StringComparison.OrdinalIgnoreCase)
							let tuple = new { ProjectPath = project.FilePath, SyntaxNode = document.GetSyntaxRoot() as SyntaxNode }
							where tuple.SyntaxNode != null
							select new { ProjectName = project.Name, EvaluationTask = _inspector.Inspect(tuple.ProjectPath, tuple.SyntaxNode) })
				.ToArray();
			await Task.WhenAll(projects.Select(_ => _.EvaluationTask));
			if (projects.Any(p => p.EvaluationTask.Exception != null))
			{
				throw new AggregateException(
					projects.Select(p => p.EvaluationTask.Exception)
							.Where(e => e != null)
							.SelectMany(e => e.InnerExceptions));
			}

			var docs = projects
				.Where(p => p.EvaluationTask.Exception == null)
				.Select(p => new { p.ProjectName, Results = p.EvaluationTask.Result.ToArray() })
				.GroupBy(p => p.ProjectName, p => p.Results)
				.Select(g => new { ProjectName = g.Key, Results = g.SelectMany(x => x).ToArray() })
				.Select(errors =>
						new EvaluationResultDocument
							{
								Id = EvaluationResultDocument.GetId(errors.ProjectName, settings.Revision.ToString(CultureInfo.InvariantCulture)), 
								ProjectName = errors.ProjectName, 
								ProjectVersion = settings.Revision.ToString(CultureInfo.InvariantCulture), 
								Results = errors.Results
							})
				.GroupBy(x => x.Id)
				.Select(g => g.First());
			using (var session = _sessionProvider.Create())
			{
				foreach (var doc in docs)
				{
					try
					{
						await session.Store(doc);
						Console.WriteLine("Store evaluation for " + doc.ProjectName);
					}
					catch (Exception exception)
					{
						Console.WriteLine("Failed to save errors for " + doc.ProjectName);
						Console.WriteLine(exception.Message);
						Console.WriteLine(exception.StackTrace);
					}
				}

				await session.Flush();
			}

			Console.WriteLine("Finished Loading Evaluation Results for " + settings.Name);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~EvaluationResultLoader()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				_projectProvider.Dispose();
				_sessionProvider.Dispose();
			}
		}
	}
}
