// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.MSBuild;

	public class SolutionProvider : IProvider<string, Task<Solution>>
	{
		private static readonly string[] SupportedProjects = { ".csproj" };
		private Dictionary<string, Task<Solution>> _cache = new Dictionary<string, Task<Solution>>();

		public SolutionProvider()
		{
			using (var workspace = new CustomWorkspace())
			{
				var solution = workspace.CurrentSolution;
				_cache.Add(string.Empty, Task.FromResult(solution));
			}
		}

		~SolutionProvider()
		{
			Dispose(false);
		}

		public Task<Solution> Get(string path)
		{
			Task<Solution> solution;
			lock (_cache)
			{
				var key = path ?? string.Empty;
				if (!_cache.ContainsKey(key))
				{
					solution = GetSolution(key).ContinueWith(x => x.Result.Item2);

					_cache.Add(key, solution);
				}
				else
				{
					solution = _cache[key];
				}
			}

			return solution;
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
				if (_cache != null)
				{
					_cache.Clear();
					_cache = null;
				}
			}
		}

		private static async Task<Tuple<int, Solution>> GetSolution(string path)
		{
			using (var workspace = MSBuildWorkspace.Create())
			{
				var solution = await workspace.OpenSolutionAsync(path).ConfigureAwait(false);
				var dependencyGraph = solution.GetProjectDependencyGraph();
				var projects = from projectid in dependencyGraph.GetTopologicallySortedProjects()
							   select solution.GetProject(projectid);

				var compilations = projects.Select(x => x.GetCompilationAsync()).ToArray();
				var count = compilations.Length;

				return new Tuple<int, Solution>(count, solution);
			}
		}
	}
}