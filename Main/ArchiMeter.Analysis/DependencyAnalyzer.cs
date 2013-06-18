namespace ArchiMeter.Analysis
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;

	public class DependencyAnalyzer
	{
		public Task<IEnumerable<DependencyChain>> GetCircularReferences(IEnumerable<EdgeItem> items)
		{
			return Task.Factory.StartNew(() => items.SelectMany(e => GetDependencyChain(new DependencyChain(Enumerable.Empty<EdgeItem>(), e, e), items))
			                                        .Where(c => c.IsCircular)
			                                        .Distinct());
		}

		private IEnumerable<DependencyChain> GetDependencyChain(DependencyChain chain,
		                                                        IEnumerable<EdgeItem> source)
		{
			return chain.IsCircular
				       ? new[] { chain }
				       : source.Where(chain.IsContinuation).SelectMany(i => GetDependencyChain(new DependencyChain(chain.ReferenceChain, chain.Root, i), source));
		}
	}
}