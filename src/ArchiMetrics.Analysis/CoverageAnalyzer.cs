// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverageAnalyzer.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CoverageAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Linq;
	using System.Threading.Tasks;
	using Common;
	using Microsoft.CodeAnalysis;

	internal class CoverageAnalyzer
	{
		private readonly Solution _solution;

		public CoverageAnalyzer(Solution solution)
		{
			_solution = solution;
		}

		public async Task<bool> IsReferencedInTest(ISymbol symbol)
		{
			var symbolReferences = await _solution.FindReferences(symbol).ConfigureAwait(false);
			if (!symbolReferences.Locations.Any())
			{
				return false;
			}

			var referencingSymbolTasks = (from location in symbolReferences.Locations
										  let rootTask = location.Location.SourceTree.GetRootAsync()
										  select new { TokenTask = rootTask, Location = location })
										 .AsArray();

			await Task.WhenAll(referencingSymbolTasks.Select(x => x.TokenTask)).ConfigureAwait(false);

			var referencingMethods = referencingSymbolTasks
				.Select(x => new
						   {
							   Token = x.TokenTask.Result.FindToken(x.Location.Location.SourceSpan.Start),
							   Model = x.Location.Model
						   })
				.Select(
					x => new
						 {
							 Method = x.Token.GetMethod(),
							 Model = x.Model,
						 })
				.AsArray();

			var referencingTests = referencingMethods
				.Select(x => x.Method)
				.Select(x => x.AttributeLists.Any(a => a.Attributes.Any(b => b.Name.ToString().IsKnownTestAttribute())));

			if (referencingTests.Any(x => x))
			{
				return true;
			}

			var referencingSymbols = from reference in referencingMethods
									 let model = reference.Model
									 let referencingSymbol = model.GetDeclaredSymbol(reference.Method)
									 select IsReferencedInTest(referencingSymbol);

			return await referencingSymbols.AsArray().FirstMatch(x => x).ConfigureAwait(false);
		}
	}
}