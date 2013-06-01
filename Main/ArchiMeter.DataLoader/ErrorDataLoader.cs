// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorDataLoader.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorDataLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.DataLoader
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Analysis;
	using CodeReview.Metrics;
	using Common;
	using Common.Documents;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class ErrorDataLoader : IDataLoader
	{
		private readonly SyntaxMetricsCalculator _calc = new SyntaxMetricsCalculator();
		private readonly SLoCCounter _counter = new SLoCCounter();

		private readonly INodeInspector _inspector;
		private readonly IProvider<string, IProject> _projectProvider;
		private readonly IFactory<IDataSession<ErrorData>> _sessionProvider;

		public ErrorDataLoader(
			INodeInspector inspector,
			IProvider<string, IProject> projectProvider, 
			IFactory<IDataSession<ErrorData>> sessionProvider)
		{
			_inspector = inspector;
			_projectProvider = projectProvider;
			_sessionProvider = sessionProvider;
		}

		public async Task Load(ProjectSettings settings)
		{
			Console.WriteLine("Loading Error Data for " + settings.Name);

			var projects = (from root in settings.Roots
							from project in _projectProvider.GetAll(root.Source)
							from document in project.Documents
							where string.Equals(Path.GetExtension(document.FilePath), ".cs", StringComparison.OrdinalIgnoreCase)
							let tuple = new { ProjectPath = project.FilePath, SyntaxNode = document.GetSyntaxRoot() as SyntaxNode }
							where tuple.SyntaxNode != null
							select new { ProjectName = project.Name, EvaluationTask = _inspector.Inspect(tuple.ProjectPath, tuple.SyntaxNode) })
				.ToArray();
			await Task.WhenAll(projects.Select(_ => _.EvaluationTask));

			var data = projects
				.Where(p => p.EvaluationTask.Exception == null)
				.Select(p => new { p.ProjectName, Results = p.EvaluationTask.Result.ToArray() })
				.SelectMany(p => p.Results.GroupBy(x => x.Comment)
				                  .Select(g =>
					                  {
						                  var snippets = g.Select(x => new Tuple<string, int>(x.Snippet, x.ErrorCount))
						                                  .GroupBy(x => x.Item1, x => x.Item2)
						                                  .Select(x => new Tuple<string, int>(x.Key, x.Sum()))
						                                  .ToArray();
						                  return new ErrorData
							                         {
								                         Id = ErrorData.GetId(g.Key, p.ProjectName, settings.Revision.ToString(CultureInfo.InvariantCulture)), 
								                         ProjectName = p.ProjectName, 
								                         ProjectVersion = settings.Revision.ToString(CultureInfo.InvariantCulture), 
								                         Error = g.Key, 
														 Category = string.Empty, 
								                         Occurrences = g.Sum(x => x.ErrorCount), 
								                         DistinctLoc = snippets.Sum(s => _counter.Count(s.Item1)), 
								                         Effort = snippets.Sum(x => GetEffort(x.Item1, x.Item2))
							                         };
					                  }))
				.GroupBy(x => x.Id)
				.Select(g => new ErrorData
					             {
						             Id = g.Key, 
						             ProjectName = g.First().ProjectName, 
						             ProjectVersion = g.First().ProjectVersion, 
						             Error = g.First().Error, 
						             Occurrences = g.Sum(x => x.Occurrences), 
						             DistinctLoc = g.Sum(x => x.DistinctLoc), 
						             Effort = g.Sum(x => x.Effort)
					             })
				.ToArray();

			using (var session = _sessionProvider.Create())
			{
				foreach (var errorData in data)
				{
					await session.Store(errorData);
				}

				await session.Flush();
				Console.WriteLine("Finished Loading Error Data for " + settings.Name);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ErrorDataLoader()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_sessionProvider.Dispose();
			}
		}

		private double GetEffort(string code, int violations)
		{
			var metrics = _calc.Calculate(code);
			var baseEffort = metrics.Sum(m => m.GetEffort().TotalSeconds);
			return Enumerable.Range(0, violations).Aggregate(0.0, (d, i) => d + (baseEffort * Math.Pow(0.5, Math.Min(3, i))));
		}
	}
}
