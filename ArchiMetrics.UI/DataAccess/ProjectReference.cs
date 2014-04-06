namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Generic;

	public class ProjectReference
	{
		public string ProjectPath { get; set; }

		public string Version { get; set; }

		public string Name { get; set; }

		public IEnumerable<string> ProjectReferences { get; set; }

		public IEnumerable<string> AssemblyReferences { get; set; }
	}
}
