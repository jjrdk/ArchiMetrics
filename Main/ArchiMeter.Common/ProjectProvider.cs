// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectProvider.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Roslyn.Services;

	public class ProjectProvider : IProvider<IProject, string>
	{
		private static readonly ConcurrentDictionary<string, IProject> Cache = new ConcurrentDictionary<string, IProject>();

		public IProject Get(string source)
		{
			return Cache.GetOrAdd(
				source,
				path =>
				{
					IWorkspace workspace = null;
					try
					{
						workspace = Workspace.LoadStandAloneProject(path, "Release", "AnyCPU");
						var project = workspace.CurrentSolution.Projects.FirstOrDefault();
						return project != null && project.HasDocuments ? project : null;
					}
					catch
					{
						Console.WriteLine("% - " + path);
						return null;
					}
					finally
					{
						if (workspace != null)
						{
							workspace.Dispose();
						}
					}
				});
		}

		public IEnumerable<IProject> GetAll(string key)
		{
			return from file in Directory.GetFiles(key, "*.csproj", SearchOption.AllDirectories)
				   where IsValid(file)
				   let p = Get(file)
				   where p != null
				   select p;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ProjectProvider()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}

		private bool IsValid(string source)
		{
			return source.IndexOf("QuickStart", StringComparison.OrdinalIgnoreCase) == -1
			&& source.IndexOf("Demo", StringComparison.OrdinalIgnoreCase) == -1;
		}
	}
}