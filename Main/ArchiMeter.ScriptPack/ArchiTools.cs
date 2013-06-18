namespace ArchiMeter.ScriptPack
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMeter.CodeReview.Metrics;
	using ArchiMeter.Common.Metrics;
	using Roslyn.Services;
	using ScriptCs.Contracts;

	public class ArchiTools : IScriptPackContext
	{
		public Task<IEnumerable<NamespaceMetric>> CalculateMetrics(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return;
			}

			path = Path.GetFullPath(path);
			var isFile = File.Exists(path);
			if (isFile)
			{
				Process.Start(path);
			}
			path = Path.GetDirectoryName(path);
			var projects = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)
				.Concat(Directory.GetFiles(path, "*.vbproj", SearchOption.AllDirectories))
				.ToArray();
			//foreach (var project in projects)
			//{
			//	Console.WriteLine(project);
			//}
			var randomFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			randomFile = Path.ChangeExtension(randomFile, "sln");
			randomFile.MergeProjectsTo(projects.ToArray());
			Process.Start(randomFile);
		}
	}
}
