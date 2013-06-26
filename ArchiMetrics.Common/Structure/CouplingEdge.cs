namespace ArchiMetrics.Common.Structure
{
	public class CouplingEdge
	{
		public string ProjectName { get; set; }
		public string ProjectVersion { get; set; }
		public string DependantNamespaceName { get; set; }
		public string DependantTypeName { get; set; }
		public string DependencyAssembly { get; set; }
		public string DependencyNamespaceName { get; set; }
		public string DependencyTypeName { get; set; }
	}
}
