// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;

	internal class ProjectMetricsCalculator : IProjectMetricsCalculator
	{
		private readonly ICodeMetricsCalculator _metricsCalculator;

		public ProjectMetricsCalculator(ICodeMetricsCalculator metricsCalculator)
		{
			_metricsCalculator = metricsCalculator;
		}

		public async Task<IEnumerable<IProjectMetric>> Calculate(Solution solution)
		{
			await solution.GetProjectDependencyGraphAsync();
			var tasks = from project in solution.Projects select Calculate(project, solution);

			return await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		public async Task<IProjectMetric> Calculate(Project project, Solution solution)
		{
			if (project == null)
			{
				return null;
			}

			var compilation = await project.GetCompilationAsync().ConfigureAwait(false);

			var metricsTask = _metricsCalculator.Calculate(project, solution);
			var referenceGraph = await solution.GetProjectDependencyGraphAsync().ConfigureAwait(false);
			var directDependants =
				referenceGraph.GetProjectsThatDirectlyDependOnThisProject(project.Id)
					.Select(x => solution.GetProject(x))
					.Select(x => x.AssemblyName);

			var referencedProjects =
				referenceGraph.GetProjectsThatThisProjectDirectlyDependsOn(project.Id)
					.Select(x => solution.GetProject(x))
					.Select(x => x.AssemblyName);
			
			var assemblyTypes = compilation.Assembly.TypeNames;
			var metrics = (await metricsTask.ConfigureAwait(false)).ToArray();

			var internalTypesUsed = metrics
				.SelectMany(x => x.ClassCouplings)
				.Where(x => x.Assembly == project.AssemblyName);
			var relationalCohesion = (internalTypesUsed.Count() + 1.0) / assemblyTypes.Count;
			
			return new ProjectMetric(project.Name, metrics, referencedProjects, directDependants, relationalCohesion);
		}
	}
}