namespace ArchiTools.ScriptPack
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using Roslyn.Services;
	using ScriptCs.Contracts;
	using ArchiMeter.Common;

	public class ArchiTools : IScriptPackContext
	{
		public void OpenProjects(string path)
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
				.Concat(Directory.GetFiles(path, "*.vbproj", SearchOption.AllDirectories));
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
