// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VertexTransformProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the VertexTransformProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;
	using Newtonsoft.Json;

	internal class VertexTransformProvider : IProvider<string, ObservableCollection<TransformRule>>
	{
		private readonly ConcurrentDictionary<string, ObservableCollection<TransformRule>> _knownRules = new ConcurrentDictionary<string, ObservableCollection<TransformRule>>();

		public VertexTransformProvider()
		{
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_knownRules.Clear();
		}

		public ObservableCollection<TransformRule> Get(string key)
		{
			return _knownRules.GetOrAdd(key, LoadRules);
		}

		public IEnumerable<ObservableCollection<TransformRule>> GetAll(string key)
		{
			return new[] { Get(key) };
		}

		private ObservableCollection<TransformRule> LoadRules(string filePath)
		{
			if (File.Exists(filePath))
			{
				using (var stream = File.OpenRead(filePath))
				using (var reader = new StreamReader(stream))
				{
					var rules = JsonConvert.DeserializeObject<List<TransformRule>>(reader.ReadToEnd());
					return new ObservableCollection<TransformRule>(rules);
				}
			}

			return new ObservableCollection<TransformRule> { new TransformRule { Name = "DotNet", Pattern = @"^(mscorlib|System)(\..+)?" } };
		}
	}
}