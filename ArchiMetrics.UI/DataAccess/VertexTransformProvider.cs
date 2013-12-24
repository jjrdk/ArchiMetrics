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
	using System.Xml.Serialization;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;

	internal class VertexTransformProvider : IProvider<string, ObservableCollection<VertexTransform>>
	{
		private readonly ConcurrentDictionary<string, ObservableCollection<VertexTransform>> _knownRules = new ConcurrentDictionary<string, ObservableCollection<VertexTransform>>();
		private readonly XmlSerializer _serializer;

		public VertexTransformProvider()
		{
			_serializer = new XmlSerializer(typeof(List<VertexTransform>));
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_knownRules.Clear();
		}

		public ObservableCollection<VertexTransform> Get(string key)
		{
			return _knownRules.GetOrAdd(key, LoadRules);
		}

		public IEnumerable<ObservableCollection<VertexTransform>> GetAll(string key)
		{
			return new[] { Get(key) };
		}

		private ObservableCollection<VertexTransform> LoadRules(string filePath)
		{
			if (File.Exists(filePath))
			{
				using (var stream = File.OpenRead(filePath))
				{
					var deserialized = _serializer.Deserialize(stream);
					var rules = (List<VertexTransform>)deserialized;
					return new ObservableCollection<VertexTransform>(rules);
				}
			}

			return new ObservableCollection<VertexTransform> { new VertexTransform { Name = "DotNet", Pattern = @"(mscorlib|System)(\..+)?" } };
		}
	}
}