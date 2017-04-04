// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProvider.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    /// <summary>
	/// Provides a concrete implementation of an <see cref="IProvider{TKey,T}"/> for loading <see cref="Solution"/>.
	/// </summary>
	public class SolutionProvider : IProvider<string, Task<Solution>>
    {
        private Dictionary<string, Task<Solution>> _cache = new Dictionary<string, Task<Solution>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionProvider"/> class.
        /// </summary>
        public SolutionProvider()
        {
            using (var workspace = new AdhocWorkspace())
            {
                var solution = workspace.CurrentSolution;
                _cache.Add(string.Empty, Task.FromResult(solution));
            }
        }

        /// <summary>
        /// Finalizes the provider.
        /// </summary>
        ~SolutionProvider()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a consistent reference to the solution at the given path.
        /// </summary>
        /// <param name="path">The file path to load the <see cref="Solution"/> from.</param>
        /// <returns>A <see cref="Task{T}"/> which will provide the solution.</returns>
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
            using (var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create())
            {
                var solution = await workspace.OpenSolutionAsync(path).ConfigureAwait(false);
                var dependencyGraph = solution.GetProjectDependencyGraph();
                var projects = dependencyGraph.GetTopologicallySortedProjects().Select(projectid => solution.GetProject(projectid));

                var compilations = projects.Select(x => x.GetCompilationAsync()).AsArray();
                var count = compilations.Length;

                return new Tuple<int, Solution>(count, solution);
            }
        }
    }
}