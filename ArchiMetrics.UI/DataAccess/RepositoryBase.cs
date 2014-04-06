namespace ArchiMetrics.UI.DataAccess
{
	using System.IO;

	public class RepositoryBase
	{
		protected RepositoryBase()
		{
		}

		protected Stream GetResourceStream(string resourceFile)
		{
			var assembly = typeof(RepositoryBase).Assembly;
			var resourceName = string.Format("{0}.Data.{1}", assembly.GetName().Name, resourceFile);
			var info = assembly.GetManifestResourceStream(resourceName);
			
			return info;
		}
	}
}
