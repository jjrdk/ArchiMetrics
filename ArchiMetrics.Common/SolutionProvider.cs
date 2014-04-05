// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.MSBuild;

	public class SolutionProvider : IProvider<string, Solution>
	{
		private ConcurrentDictionary<string, Solution> _cache = new ConcurrentDictionary<string, Solution>();

		public SolutionProvider()
		{
			using (var workspace = new CustomWorkspace(SolutionId.CreateNewId("empty")))
			{
				_cache.TryAdd(string.Empty, workspace.CurrentSolution);
			}
		}

		~SolutionProvider()
		{
			Dispose(false);
		}

		public Solution Get(string path)
		{
			var solution = _cache.GetOrAdd(
				path ?? string.Empty,
				p =>
				{
					using (var workspace = MSBuildWorkspace.Create())
					{
						return workspace.OpenSolutionAsync(p)
							.Result;
					}
				});

			return solution;
		}

		public IEnumerable<Solution> GetAll(string key)
		{
			return string.IsNullOrWhiteSpace(key)
				? Enumerable.Empty<Solution>()
				: (from file in Directory.GetFiles(key, "*.sln", SearchOption.AllDirectories)
				   let s = Get(file)
				   where s != null
				   select s);
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
				if (_cache != null)
				{
					_cache.Clear();
					_cache = null;
				}
			}
		}
	}
}