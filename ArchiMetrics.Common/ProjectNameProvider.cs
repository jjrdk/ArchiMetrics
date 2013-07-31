// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectNameProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectNameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System;
	using System.Collections.Generic;
	using Roslyn.Services;

	public class ProjectNameProvider : IProvider<string, string>
	{
		private readonly IProvider<string, IProject> _projectProvider;

		public ProjectNameProvider(IProvider<string, IProject> projectProvider)
		{
			_projectProvider = projectProvider;
		}

		~ProjectNameProvider()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public string Get(string key)
		{
			var project = _projectProvider.Get(key);
			if (project != null)
			{
				return project.Name;
			}

			return string.Empty;
		}

		public IEnumerable<string> GetAll(string key)
		{
			throw new NotImplementedException();
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
			}
		}
	}
}
