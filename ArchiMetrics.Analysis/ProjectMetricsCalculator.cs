// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.Metrics;
    using ArchiMetrics.Common.Metrics;
    using Roslyn.Services;

    internal class ProjectMetricsCalculator
    {
        private readonly CodeMetricsCalculator _codeMetricsCalculator;

        public ProjectMetricsCalculator()
        {
            _codeMetricsCalculator = new CodeMetricsCalculator();
        }

        public async Task<IEnumerable<IProjectMetric>> Calculate(ISolution solution)
        {
            var metrics = (from project in solution.Projects
                           let codeMetricsTask = _codeMetricsCalculator.Calculate(project)
                           let projectDependencies =
                               project.ProjectReferences.Select(solution.GetProject)
                                   .Where(p => p != null)
                                   .Select(p => p.AssemblyName)
                           let metaReferences = project.MetadataReferences.Select(m => m.Display)
                           select
                               new
                               {
                                   Name = project.AssemblyName,
                                   Metrics = codeMetricsTask,
                                   References = projectDependencies.Concat(metaReferences).ToArray()
                               }).ToArray();

            await Task.WhenAll(metrics.Select(x => x.Metrics));

            return metrics.Select(x => new ProjectMetric(x.Name, x.Metrics.Result, x.References));
        }
    }
}