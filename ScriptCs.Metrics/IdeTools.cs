namespace ScriptCs.Metrics
{
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using ArchiMeter.Common;
	using ScriptCs.Contracts;

	public class IdeTools : IScriptPackContext
	{
		public void OpenProjects(string path)
		{
			if (CheckSinglePath(ref path))
			{
				return;
			}
			path = Path.GetDirectoryName(path);
			var projects = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)
				.Concat(Directory.GetFiles(path, "*.vbproj", SearchOption.AllDirectories))
				.ToArray();

			var randomFile = CreateRandomFileName();
			randomFile.MergeProjectsTo(projects);
			Process.Start(randomFile);
		}

		public void OpenSolutions(string path)
		{
			if (CheckSinglePath(ref path))
			{
				return;
			}
			path = Path.GetDirectoryName(path);
			var solutions = Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories).ToArray();

			var randomFile = CreateRandomFileName();
			randomFile.MergeSolutionsTo(solutions);
			Process.Start(randomFile);
		}

		private static string CreateRandomFileName()
		{
			var randomFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			randomFile = Path.ChangeExtension(randomFile, "sln");
			return randomFile;
		}

		private static bool CheckSinglePath(ref string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return true;
			}

			path = Path.GetFullPath(path);
			var isFile = File.Exists(path);
			if (isFile)
			{
				Process.Start(path);
			}
			return false;
		}
	}
}
