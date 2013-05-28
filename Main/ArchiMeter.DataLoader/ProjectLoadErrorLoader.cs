// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLoadErrorLoader.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectLoadErrorLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.DataLoader
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;

	using Roslyn.Services;

	public class ProjectLoadErrorLoader : IDataLoader
	{
		private readonly IFactory<IDataSession<ProjectLoadErrorDocument>> _sessionFactory;

		public ProjectLoadErrorLoader(IFactory<IDataSession<ProjectLoadErrorDocument>> sessionFactory)
		{
			this._sessionFactory = sessionFactory;
		}

		public async Task Load(ProjectSettings settings)
		{
			var failures = settings.Roots
								   .Select(root => root.Source)
								   .SelectMany(this.GetFailedProjects)
								   .Where(ex => ex != null)
								   .Select(f => new LoadErrorDetailsDocument
												  {
													  ErrorMessage = f.Item2.Message, 
													  StackTrace = f.Item2.StackTrace, 
													  ProjectPath = f.Item1
												  });

			using (var session = this._sessionFactory.Create())
			{
				var document = new ProjectLoadErrorDocument
								   {
									   Id = ProjectLoadErrorDocument.GetId(settings.Name, settings.Revision), 
									   ProjectName = settings.Name, 
									   ProjectVersion = settings.Revision, 
									   Details = failures.ToArray()
								   };

				await session.Store(document);
				await session.Flush();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ProjectLoadErrorLoader()
		{
			// Simply call Dispose(false).
			this.Dispose(false);
		}

		public IEnumerable<Tuple<string, Exception>> GetFailedProjects(string key)
		{
			return from file in Directory.GetFiles(key, "*.csproj", SearchOption.AllDirectories)
				   where this.IsValid(file)
				   let exception = this.GetProjectLoadException(file)
				   where exception != null
				   select new Tuple<string, Exception>(file, exception);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
			}
		}

		private Exception GetProjectLoadException(string path)
		{
			IWorkspace workspace = null;
			try
			{
				workspace = Workspace.LoadStandAloneProject(path, "Release", "AnyCPU");
				var project = workspace.CurrentSolution.Projects.FirstOrDefault();
				var hasDocuments = project.HasDocuments;
				return null;
			}
			catch (Exception exception)
			{
				return exception;
			}
			finally
			{
				if (workspace != null)
				{
					workspace.Dispose();
				}
			}
		}

		private bool IsValid(string source)
		{
			return source.IndexOf("QuickStart", StringComparison.OrdinalIgnoreCase) == -1
			&& source.IndexOf("Demo", StringComparison.OrdinalIgnoreCase) == -1;
		}
	}
}