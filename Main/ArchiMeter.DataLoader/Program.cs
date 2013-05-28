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
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Xml.Serialization;
	using Analysis;
	using Autofac;
	using CodeReview;
	using CodeReview.Metrics;
	using Common;
	using Common.Documents;
	using Common.Metrics;
	using Data.DataAccess;
	using Ionic.Zip;
	using NHunspell;
	using Raven;
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
			foreach (var type in typeof(ICodeEvaluation).Assembly
				.GetTypes()
				.Where(t => typeof(ICodeEvaluation).IsAssignableFrom(t))
				.Where(t => !t.IsInterface && !t.IsAbstract))
			{
				builder.RegisterType(type)
					.As<ICodeEvaluation>();
			}
			using (var dictFile = ZipFile.Read(@"Dictionaries\dict-en.oxt"))
			{
				var affStream = new MemoryStream();
				var dicStream = new MemoryStream();
				dictFile.FirstOrDefault(z => z.FileName == "en_US.aff")
					.Extract(affStream);
				dictFile.FirstOrDefault(z => z.FileName == "en_US.dic")
					.Extract(dicStream);
				builder.RegisterInstance(new Hunspell(affStream.ToArray(), dicStream.ToArray()));
			}
			builder.RegisterType<KnownWordList>()
				.As<IKnownWordList>();
			builder.RegisterType<SpellChecker>()
				.As<ISpellChecker>();
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
				   .As<IProvider<string, IProject>>();
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
