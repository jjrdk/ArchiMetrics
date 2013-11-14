// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCollectionCopier.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultCollectionCopier type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Support
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;

	internal class DefaultCollectionCopier : ICollectionCopier
	{
		private readonly BinaryFormatter _formatter = new BinaryFormatter();

		public async Task<IEnumerable<T>> Copy<T>(IEnumerable<T> source, CancellationToken cancellationToken)
		{
			if (source == null)
			{
				return Enumerable.Empty<T>();
			}

			using (var memoryStream = new MemoryStream())
			{
				_formatter.Serialize(memoryStream, source.ToArray());
				await memoryStream.FlushAsync(cancellationToken);
				memoryStream.Seek(0, SeekOrigin.Begin);
				return cancellationToken.IsCancellationRequested
					? Enumerable.Empty<T>()
					: (T[])_formatter.Deserialize(memoryStream);
			}
		}
	}
}
