// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.ExcelWriter
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Xml.Serialization;
	using Analysis;
	using Autofac;
	using CodeReview;
	using Common;
	using Common.Documents;
	using Common.Metrics;
	using Data.DataAccess;
	using global::Raven.Client;
	using Raven;
	using Raven.Repositories;
	using Reports;
	using Roslyn.Services;

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

			var config = new XmlSerializer(
							 typeof(ReportConfig),
							 new[] { typeof(ProjectSettings), typeof(ProjectDefinition) })
							 .Deserialize(File.OpenRead(configPath)) as ReportConfig;
			var builder = new ContainerBuilder();
			builder.RegisterInstance(new NamedDocumentStoreProvider(config.DatabaseUrl, config.ApiKey))
				   .As<IProvider<IDocumentStore>>();
			builder.RegisterType<ExcelReportWriter>();
			builder.RegisterType<SlocCounter>();
			builder.RegisterInstance(new PathFilter(ReportUtils.AllCode));
			builder.RegisterType<ErrorDataProviderFactory>()
				   .As<IFactory<Func<ProjectInventoryDocument, string[]>, ErrorDataProvider>>();
			builder.RegisterType<MetricsProviderFactory>()
				   .As<IFactory<Func<ProjectInventoryDocument, string[]>, MetricsProvider>>();
			builder.RegisterType<SolutionInspector>()
				   .As<INodeInspector>();
			builder.RegisterType<AsyncSessionFactory>().SingleInstance()
				   .As<IFactory<IAsyncDocumentSession>>();
			builder.RegisterType<SessionFactory>().SingleInstance()
				   .As<IFactory<IDocumentSession>>();
			builder.RegisterType<TypeCouplingProvider>()
				   .As<IAsyncProvider<string, string, TypeCoupling>>();
			builder.RegisterType<TypeSizeRepository>()
				   .As<IAsyncReadOnlyRepository<TypeSizeSegment>>();
			builder.RegisterType<TypeSizeComplexityGeoMeanRepository>()
				   .As<IAsyncReadOnlyRepository<TypeSizeComplexityGeoMeanSegment>>();
			builder.RegisterType<TypeComplexityRepository>()
				   .As<IAsyncReadOnlyRepository<TypeComplexitySegment>>();
			builder.RegisterType<TypeMaintainabilityRepository>()
				   .As<IAsyncReadOnlyRepository<TypeMaintainabilitySegment>>();
			builder.RegisterType<SizeComplexityScatterRepository>()
				   .As<IAsyncReadOnlyRepository<MemberSizeComplexitySegment>>();
			builder.RegisterType<SizeMaintainabilityScatterRepository>()
				   .As<IAsyncReadOnlyRepository<MemberSizeMaintainabilitySegment>>();
			builder.RegisterType<ComplexityMaintainabilityScatterRepository>()
				   .As<IAsyncReadOnlyRepository<MemberComplexityMaintainabilitySegment>>();
			builder.RegisterType<MetricsRepository>()
				   .As<IDataSession<ProjectMetricsDocument>>()
				   .As<IReadOnlyDataSession<ProjectMetricsDocument>>();
			builder.RegisterType<ErrorSnippetRepositoryFactory>()
				   .As<IFactory<IReadOnlyDataSession<CodeErrors>>>();
			builder.RegisterType<MetricsRepositoryFactory>()
				   .As<IFactory<IDataSession<ProjectMetricsDocument>>>();
			builder.RegisterType<EvaluationRepositoryFactory>()
				   .As<IFactory<IDataSession<EvaluationResultDocument>>>();
			builder.RegisterType<SolutionEdgeItemsRepositoryConfig>()
				   .As<ISolutionEdgeItemsRepositoryConfig>();
			builder.RegisterType<ProjectProvider>()
				   .As<IProvider<string, IProject>>()
				   .InstancePerDependency();
			builder.RegisterType<RequirementTestAnalyzer>()
				   .As<IRequirementTestAnalyzer>();

			//builder.RegisterType<TypeCouplingReport>()
			//	  .As<IReportJob>();
			//builder.RegisterType<ProjectLoadErrorReport>().As<IReportJob>();
			//builder.RegisterType<TestMetricsReport>()
			//	  .As<IReportJob>();
			//builder.RegisterType<TestCodeReviewReport>()
			//	  .As<IReportJob>();
			//builder.RegisterType<ProductionMetricsReport>()
			//	  .As<IReportJob>();
			//builder.RegisterType<ProductionCodeReviewReport>()
			//	  .As<IReportJob>();
			//builder.RegisterType<TypeSizeComplexityGeoMeanDistributionReport>()
			//	   .As<IReportJob>();
			//builder.RegisterType<TypeSizeDistributionReport>()
			//	   .As<IReportJob>();
			//builder.RegisterType<TypeComplexityDistributionReport>()
			//	   .As<IReportJob>();
			//builder.RegisterType<TypeMaintainabilityDistributionReport>()
			//	   .As<IReportJob>();
			//builder.RegisterType<NamespaceMaintainabilityDeviationReport>()
			//	   .As<IReportJob>();
			builder.RegisterType<SizeComplexityScatterReport>()
				   .As<IReportJob>();
			builder.RegisterType<SizeMaintainabilityScatterReport>()
				   .As<IReportJob>();
			builder.RegisterType<ComplexityMaintainabilityScatterReport>()
				   .As<IReportJob>();

			// builder.RegisterType<AssertionReport>().As<IReportJob>();
			// builder.RegisterType<ModelComparisonReport>().As<IReportJob>();
			// builder.RegisterType<RequirementsReport>().As<IReportJob>();
			var container = builder.Build();

			var stopwatch = new Stopwatch();
			var writer = container.Resolve<ExcelReportWriter>();
			Console.WriteLine("Starting report generation");
			stopwatch.Start();
			var task = writer.GenerateReport(config)
							 .ContinueWith(t =>
											   {
												   stopwatch.Stop();
												   if (t.Exception != null)
												   {
													   foreach (var exception in t.Exception.InnerExceptions)
													   {
														   Console.WriteLine(exception.Message);
													   }
												   }

												   Console.WriteLine("Generated report in " + stopwatch.Elapsed);
												   Console.WriteLine("Press Enter to continue.");
											   });
			task.Wait();
			Console.ReadLine();
		}
	}
}
