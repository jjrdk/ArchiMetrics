// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCollectionCopier.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
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
	using System.Threading.Tasks;
	using Common;

	internal class DefaultCollectionCopier : ICollectionCopier
	{
		private readonly BinaryFormatter _formatter = new BinaryFormatter();

		public async Task<IEnumerable<T>> Copy<T>(IEnumerable<T> source)
		{
			using (var memoryStream = new MemoryStream())
			{
				_formatter.Serialize(memoryStream, source.ToArray());
				await memoryStream.FlushAsync();
				memoryStream.Seek(0, SeekOrigin.Begin);
				return (T[])_formatter.Deserialize(memoryStream);
			}
		}
	}
}