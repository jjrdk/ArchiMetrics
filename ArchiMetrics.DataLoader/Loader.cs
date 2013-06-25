// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Loader.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Loader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.DataLoader
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Common;

	internal class Loader
	{
		private readonly IEnumerable<IDataLoader> _loaders;

		public Loader(IEnumerable<IDataLoader> loaders)
		{
			_loaders = loaders;
		}

		public async Task LoadData(ReportConfig config)
		{
			var work = config.Projects.SelectMany(p => _loaders.Select(l => new { Settings = p, Loader = l }));

			foreach (var tuple in work)
			{
				await tuple.Loader.Load(tuple.Settings);
			}
		}
	}
}
