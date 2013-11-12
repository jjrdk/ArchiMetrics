// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Roslyn.Services;

	public class ProjectProvider : IProvider<string, IProject>
	{
		private static readonly ConcurrentDictionary<string, IProject> Cache = new ConcurrentDictionary<string, IProject>();

		~ProjectProvider()
		{
			Dispose(false);
		}

		public IProject Get(string source)
		{
			return Cache.GetOrAdd(
				source, 
				path =>
				{
					try
					{
						var project = Solution.LoadStandAloneProject(path, "Release");
						return project != null && project.HasDocuments ? project : null;
					}
					catch
					{
						return null;
					}
				});
		}

		public IEnumerable<IProject> GetAll(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return Enumerable.Empty<IProject>();
			}

			return from file in Directory.GetFiles(key, "*.csproj", SearchOption.AllDirectories)
				   let p = Get(file)
				   where p != null
				   select p;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}
	}
}
