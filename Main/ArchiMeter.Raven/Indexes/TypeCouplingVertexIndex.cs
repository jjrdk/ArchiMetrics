namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using Common.Structure;
	using global::Raven.Client.Indexes;

	public class TypeCouplingVertexIndex : AbstractIndexCreationTask<ProjectMetricsDocument, CouplingEdge>
	{
		public TypeCouplingVertexIndex()
		{
			Map = docs => from doc in docs
				from nm in doc.Metrics
				from tm in nm.TypeMetrics
				let tmc = tm.ClassCouplings
				from coupling in tmc
				select new 
					   {
						   ProjectName = doc.ProjectName,
						   ProjectVersion = doc.ProjectVersion,
						   DependantNamespaceName = nm.Name,
						   DependantTypeName = tm.Name,
						   DependencyAssembly = coupling.Assembly,
						   DependencyNamespaceName = coupling.Namespace,
						   DependencyTypeName = coupling.ClassName
					   };
		}
	}
}