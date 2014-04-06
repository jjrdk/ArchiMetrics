namespace ArchiMetrics.Common.Structure
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IVertexRepository
	{
		Task<IEnumerable<IModelNode>> GetVertices(string solutionPath, CancellationToken cancellationToken);
	}
}
