namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Generic;

	public class NamespaceReference
	{
		public string ProjectPath { get; set; }

		public string ProjectVersion { get; set; }

		public string Namespace { get; set; }

		public IEnumerable<string> References { get; set; }
	}
}
