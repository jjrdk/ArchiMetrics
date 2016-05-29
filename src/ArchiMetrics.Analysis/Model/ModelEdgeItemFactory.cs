// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelEdgeItemFactory.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelEdgeItemFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Common;
	using Common.Structure;

    internal class ModelEdgeItemFactory : IAsyncFactory<IEnumerable<IModelNode>, IEnumerable<ModelEdgeItem>>
	{
		~ModelEdgeItemFactory()
		{
			Dispose(false);
		}

		public Task<IEnumerable<ModelEdgeItem>> Create(IEnumerable<IModelNode> memberSymbol, CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(
				() => memberSymbol
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
