using System.IO;
using System.Xml.Serialization;

namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;

	internal class VertexTransformProvider : IProvider<string, IEnumerable<VertexTransform>>
	{
		private readonly ConcurrentDictionary<string, IEnumerable<VertexTransform>> _knownRules = new ConcurrentDictionary<string, IEnumerable<VertexTransform>>();
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

		public IEnumerable<VertexTransform> Get(string key)
		{
			return _knownRules.GetOrAdd(key, LoadRules);
		}

		public IEnumerable<IEnumerable<VertexTransform>> GetAll(string key)
		{
			return new[] { Get(key) };
		}

		private IEnumerable<VertexTransform> LoadRules(string filePath)
		{
			if (File.Exists(filePath))
			{
				using (var stream = File.OpenRead(filePath))
				{
					var deserialized = _serializer.Deserialize(stream);
					var rules = (List<VertexTransform>)deserialized;
					return rules;
				}
			}
			return new[] { new VertexTransform { Name = "DotNet", Pattern = @"(mscorlib|System)(\..+)?" } };
		}
	}
}