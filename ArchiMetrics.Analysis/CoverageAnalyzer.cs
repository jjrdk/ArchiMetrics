namespace ArchiMetrics.Analysis
{
	using System.Linq;
	using ArchiMetrics.Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Services;

	public class CoverageAnalyzer
	{
		private readonly ISolution _solution;

		public CoverageAnalyzer(ISolution solution)
		{
			_solution = solution;
		}

		public bool IsReferencedInTest(ISymbol symbol)
		{
			var references = symbol.FindReferences(_solution).ToArray();
			if (!references.Any())
			{
				return false;
			}

			var referencingMethods = references.SelectMany(
				x => x.Locations.Select(
					y => new
						 {
							 Token = y.Document.GetSyntaxRoot().FindToken(y.Location.SourceSpan.Start),
							 Document = y.Document,
						 }))
				.Select(
					x => new
						 {
							 Method = x.Token.GetMethod(),
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

			var referencingSymbols = from reference in referencingMethods
									 let model = reference.Document.GetSemanticModel()
									 let referencingSymbol = model.GetDeclaredSymbol(reference.Method)
									 select IsReferencedInTest(referencingSymbol);

			return referencingSymbols.Any(x => x);
		}
	}
}