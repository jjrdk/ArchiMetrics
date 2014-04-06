namespace ArchiMetrics.Common.Structure
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface ISyntaxTransformer
	{
		Task<IEnumerable<IModelNode>> Transform(IEnumerable<IModelNode> source, IEnumerable<TransformRule> rules, CancellationToken cancellationToken);
	}
}