// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInventoryLoader.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectInventoryLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.DataLoader
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using CodeReview;
	using Common;
	using Common.Documents;
	using Roslyn.Services;

	public class ProjectInventoryLoader : IDataLoader
	{
		private readonly IProvider<string, IProject> _projectProvider;
		private readonly IFactory<IDataSession<ProjectInventoryDocument>> _sessionProvider;

		public ProjectInventoryLoader(
			IProvider<string, IProject> projectProvider, 
			IFactory<IDataSession<ProjectInventoryDocument>> sessionProvider)
		{
			_projectProvider = projectProvider;
			_sessionProvider = sessionProvider;
		}

		public async Task Load(ProjectSettings settings)
		{
			Console.WriteLine("Loading Inventory for " + settings.Name);

			var testProjectNames = GetNames(settings, ReportUtils.TestCode);
			var productionProjectNames = GetNames(settings, ReportUtils.ProductionCode);
			var doc = new ProjectInventoryDocument
						  {
							  Id = ProjectInventoryDocument.GetId(settings.Name, settings.Revision.ToString(CultureInfo.InvariantCulture)), 
							  Tag = settings.Tag,
							  Date = settings.Date,
							  ProductionProjectNames = productionProjectNames.Distinct().ToArray(), 
							  TestProjectNames = testProjectNames.Distinct().ToArray(), 
							  ProjectName = settings.Name, 
							  ProjectVersion = settings.Revision.ToString(CultureInfo.InvariantCulture)
						  };

			using (var session = _sessionProvider.Create())
			{
				var text = "Finished Loading Inventory for " + settings.Name;
				await session.Store(doc);
				await session.Flush();
				Console.WriteLine(text);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ProjectInventoryLoader()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				_projectProvider.Dispose();
				_sessionProvider.Dispose();
			}
		}

		private IEnumerable<string> GetNames(ProjectSettings settings, Func<ProjectDefinition, bool> filter)
		{
			return from root in settings.Roots
				   from file in Directory.GetFiles(root.Source, "*.csproj", SearchOption.AllDirectories)
				   where filter(new ProjectDefinition { IsTest = root.IsTest, Source = file })
				   let project = _projectProvider.Get(file)
				   where project != null
				   select project.Name;
		}
	}
}
