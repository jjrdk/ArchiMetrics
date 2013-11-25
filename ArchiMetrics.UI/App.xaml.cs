// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the App type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI
{
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Windows;
	using System.Windows.Markup;
	using System.Windows.Threading;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.CodeReview.Rules;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.DataAccess;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using Autofac;
	using Ionic.Zip;
	using NHunspell;
	using Roslyn.Services;

	public partial class App : Application
	{
		static App()
		{
			FrameworkElement.LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
		}

		public App()
		{
			DispatcherUnhandledException += OnDispatcherUnhandledException;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			DispatcherUnhandledException -= OnDispatcherUnhandledException;
			base.OnExit(e);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var container = BuildContainer();
			var loader = new ModernContentLoader(container);
			Resources.Add("Loader", loader);
			base.OnStartup(e);
		}

		private static IContainer BuildContainer()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<DefaultCollectionCopier>()
				   .As<ICollectionCopier>()
				   .SingleInstance();

			using (var dictFile = ZipFile.Read(@"Dictionaries\dict-en.oxt"))
			{
				var affStream = new MemoryStream();
				var dicStream = new MemoryStream();
				dictFile.First(z => z.FileName == "en_US.aff").Extract(affStream);
				dictFile.First(z => z.FileName == "en_US.dic").Extract(dicStream);
				builder.RegisterInstance(new Hunspell(affStream.ToArray(), dicStream.ToArray()));
				affStream.Dispose();
				dicStream.Dispose();
			}

			var evaluationTypes = from type in typeof(ReportUtils).Assembly.GetTypes()
								  where typeof(ICodeEvaluation).IsAssignableFrom(type)
								  where !type.IsInterface && !type.IsAbstract
								  select type;

			foreach (var type in evaluationTypes)
			{
				builder.RegisterType(type).As<IEvaluation>();
			}

			builder.RegisterType<SpellChecker>().As<ISpellChecker>();
			builder.RegisterType<KnownPatterns>().As<IKnownPatterns>();
			builder.RegisterType<CodeMetricsCalculator>().As<ICodeMetricsCalculator>();
			builder.RegisterType<NodeReviewer>().As<INodeInspector>();
			builder.RegisterType<MetricsRepository>().As<IProjectMetricsRepository>().SingleInstance();
			var vertexRuleRepository = new VertexRuleRepository();
			builder.RegisterInstance(new PathFilter(x => true)).As<PathFilter>();
			builder.RegisterType<SolutionProvider>().As<IProvider<string, ISolution>>().SingleInstance();
			builder.RegisterType<ProjectProvider>().As<IProvider<string, IProject>>().SingleInstance();
			builder.RegisterType<CodeErrorRepository>().As<ICodeErrorRepository>().SingleInstance();
			builder.RegisterType<AggregateEdgeItemsRepository>().As<IEdgeItemsRepository>().SingleInstance();
			builder.RegisterInstance<IVertexRuleRepository>(vertexRuleRepository);
			builder.RegisterInstance<IVertexRuleDefinition>(vertexRuleRepository);
			builder.RegisterType<EdgeTransformer>().As<IEdgeTransformer>();
			builder.RegisterType<RequirementTestAnalyzer>().As<IRequirementTestAnalyzer>();
			builder.RegisterType<EdgesViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<CircularReferenceViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<CodeErrorGraphViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<CodeReviewViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<GraphViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<RequirementGraphViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<TestErrorGraphViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<SettingsViewModel>().As<ViewModelBase>().AsSelf();
			builder.RegisterType<AvailableRules>().As<IAvailableRules>().SingleInstance();
			builder.RegisterType<AppContext>()
				.As<IAppContext>()
				.SingleInstance();
			var container = builder.Build();

			return container;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message);
			Shutdown(1);
		}
	}
}
