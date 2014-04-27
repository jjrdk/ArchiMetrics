// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoverageAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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
	using ArchiMetrics.Common;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.FindSymbols;

	public class CoverageAnalyzer
	{
		private readonly Solution _solution;

		public CoverageAnalyzer(Solution solution)
		{
			_solution = solution;
		}

		public async Task<bool> IsReferencedInTest(ISymbol symbol)
		{
			var references = (await SymbolFinder.FindReferencesAsync(symbol, _solution).ConfigureAwait(false)).ToArray(); // symbol.FindReferences(_solution).ToArray();
			if (!references.Any())
			{
				return false;
			}

			var referencingSymbolTasks = (from reference in references
										  from location in reference.Locations
										  let rootTask = location.Document.GetSyntaxRootAsync()
										  select new { TokenTask = rootTask, Location = location })
										 .ToArray();

			await Task.WhenAll(referencingSymbolTasks.Select(x => x.TokenTask)).ConfigureAwait(false);

			var referencingMethods = referencingSymbolTasks
				.Select(x => new
						   {
							   Token = x.TokenTask.Result.FindToken(x.Location.Location.SourceSpan.Start), 
							   Document = x.Location.Document
						   })
				.Select(
					x => new
						 {
							 Method = x.Token.GetMethod(), 
							 Model = x.Document.GetSemanticModelAsync(), 
							 Document = x.Document
						 })
				.ToArray();

			var referencingTests = referencingMethods
				.Select(x => x.Method)
				.Select(x => x.AttributeLists.Any(a => a.Attributes.Any(b => b.Name.ToString().IsKnownTestAttribute())));

			if (referencingTests.Any(x => x))
			{
				return true;
			}

			await Task.WhenAll(referencingMethods.Select(x => x.Model)).ConfigureAwait(false);
			var referencingSymbols = from reference in referencingMethods
									 let model = reference.Model.Result
									 let referencingSymbol = model.GetDeclaredSymbol(reference.Method)
									 select IsReferencedInTest(referencingSymbol);

			return await referencingSymbols.ToArray().FirstMatch(x => x).ConfigureAwait(false);
		}
	}
}