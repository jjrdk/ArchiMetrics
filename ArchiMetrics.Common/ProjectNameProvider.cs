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

		~ProjectNameProvider()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				//Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.

			}
		}
	}
}
