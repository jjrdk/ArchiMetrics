namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;

	internal class ModelEdgeItemFactory : IAsyncFactory<IEnumerable<IModelNode>, IEnumerable<ModelEdgeItem>>
	{
		~ModelEdgeItemFactory()
		{
			Dispose(false);
		}

		public Task<IEnumerable<ModelEdgeItem>> Create(IEnumerable<IModelNode> nodes, CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(
				() => nodes
					.SelectMany(x => x.Flatten())
					.WhereNot(x => string.IsNullOrWhiteSpace(x.QualifiedName))
					.SelectMany(x => x.Children.Select(y => new ModelEdgeItem(x, y))),
				cancellationToken);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}
	}
}
