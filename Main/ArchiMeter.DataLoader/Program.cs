// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.DataLoader
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Xml.Serialization;
	using Analysis;
	using ArchiMeter.Common.Documents;
	using Autofac;
	using CodeReview;
	using CodeReview.Metrics;
	using Common;
	using Common.Metrics;
	using Data.DataAccess;
	using Raven;
	using Raven.Loading;
	using Raven.Repositories;
	using Roslyn.Services;
	using Tfs;
	using global::Raven.Client;

	class Program
	{
		static void Main(string[] args)
		{
			var configPath = args.FirstOrDefault() ?? "settings.xml";
			if (!File.Exists(configPath))
			{
				Console.WriteLine("No config file found.");
				Console.ReadLine();
				return;
			}

			var config = new XmlSerializer(typeof(ReportConfig), new[] { typeof(ProjectSettings), typeof(ProjectDefinition) })
							 .Deserialize(File.OpenRead(configPath)) as ReportConfig;
			var builder = new ContainerBuilder();
			builder.RegisterInstance(config);
			builder.RegisterInstance(new EmbeddedDocumentStoreProvider())
				   .As<IProvider<IDocumentStore>>();
			builder.RegisterType<Loader>();
			builder.RegisterType<SLoCCounter>();
			builder.RegisterInstance(new PathFilter(ReportUtils.AllCode));
			builder.RegisterType<SolutionInspector>()
				   .As<INodeInspector>();
			builder.Register(x => DefinedRules.Default)
				   .As<IEnumerable<ICodeEvaluation>>();
			builder.RegisterType<AsyncSessionFactory>()
				   .As<IFactory<IAsyncDocumentSession>>()
				   .SingleInstance();
			builder.RegisterType<MetricsRepository>()
				   .As<IDataSession<ProjectMetricsDocument>>()
				   .As<IReadOnlyDataSession<ProjectMetricsDocument>>();
			builder.RegisterType<MetricsRepositoryFactory>()
				   .As<IFactory<IDataSession<ProjectMetricsDocument>>>();
			builder.RegisterType<GenericSessionFactory<ProjectInventoryDocument>>()
				   .As<IFactory<IDataSession<ProjectInventoryDocument>>>();
			builder.RegisterType<GenericSessionFactory<ProjectLoadErrorDocument>>()
				   .As<IFactory<IDataSession<ProjectLoadErrorDocument>>>();
			builder.RegisterType<GenericSessionFactory<ErrorData>>()
				   .As<IFactory<IDataSession<ErrorData>>>();
			builder.RegisterType<GenericSessionFactory<TfsMetricsDocument>>()
				   .As<IFactory<IDataSession<TfsMetricsDocument>>>();
			builder.RegisterType<EvaluationRepositoryFactory>()
				   .As<IFactory<IDataSession<EvaluationResultDocument>>>();
			builder.RegisterType<ProjectMetricsCalculator>()
				   .As<IProjectMetricsCalculator>();
			builder.RegisterType<SolutionEdgeItemsRepositoryConfig>()
				   .As<ISolutionEdgeItemsRepositoryConfig>();
			builder.RegisterType<ProjectProvider>()
				   .As<IProvider<IProject, string>>();
			builder.RegisterType<RequirementTestAnalyzer>()
				   .As<IRequirementTestAnalyzer>();
			builder.RegisterType<Exporter>();
			builder.RegisterInstance(new SqlConnectionFactory(config.TfsConnectionString))
				.As<IFactory<SqlConnection>>();
			//builder.RegisterType<ProjectLoadErrorLoader>().As<IDataLoader>();
			//builder.RegisterType<ErrorDataLoader>().As<IDataLoader>();
			//builder.RegisterType<EvaluationResultLoader>().As<IDataLoader>();
			//builder.RegisterType<ProjectMetricsLoader>().As<IDataLoader>();
			//builder.RegisterType<ProjectInventoryLoader>().As<IDataLoader>();
			builder.RegisterType<TfsMetricsLoader>().As<IDataLoader>();

			var container = builder.Build();

			var stopwatch = new Stopwatch();
			var writer = container.Resolve<Loader>();
			Console.WriteLine("Starting data load.");
			stopwatch.Start();
			var task = writer.LoadData(config)
							 .ContinueWith(t =>
											   {
												   stopwatch.Stop();
												   WriteExceptions(t);
											   })
								 .ContinueWith(t =>
									 {
										 Console.WriteLine("Exporting Data");
										 var exporter = container.Resolve<Exporter>();
										 exporter.Export();
										 Console.WriteLine("Finished Exporting Data");
									 })
							 .ContinueWith(t =>
								 {
									 WriteExceptions(t);
									 Console.WriteLine("Data loaded in " + stopwatch.Elapsed);
									 Console.WriteLine("Press Enter to continue.");
								 });
			task.Wait();
			Console.ReadLine();
		}

		private static void WriteExceptions(Task t)
		{
			if (t.Exception == null)
			{
				return;
			}

			foreach (var exception in t.Exception.InnerExceptions)
			{
				Console.WriteLine(exception.Message);
				Console.WriteLine(exception.StackTrace);
			}
		}
	}
}
