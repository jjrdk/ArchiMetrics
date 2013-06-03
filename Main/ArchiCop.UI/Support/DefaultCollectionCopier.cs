// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCollectionCopier.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultCollectionCopier type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.Support
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading.Tasks;

	using ArchiMeter.Common;

	internal class DefaultCollectionCopier : ICollectionCopier
	{
		private readonly BinaryFormatter _formatter = new BinaryFormatter();

		public async Task<IEnumerable<T>> Copy<T>(IEnumerable<T> source)
		{
			using (var memoryStream = new MemoryStream())
			{
				this._formatter.Serialize(memoryStream, source.ToArray());
				await memoryStream.FlushAsync();
				memoryStream.Seek(0, SeekOrigin.Begin);
				return (T[])this._formatter.Deserialize(memoryStream);
			}
		}
	}
}