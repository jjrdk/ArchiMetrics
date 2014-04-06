namespace ArchiMetrics.UI
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reactive.Concurrency;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Windows;
	using System.Windows.Markup;
	using System.Windows.Threading;
	using ArchiMetrics.Analysis;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Analysis.Model;
	using ArchiMetrics.Analysis.Validation;
	using ArchiMetrics.CodeReview.Rules;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.DataAccess;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.Support.Messages;
	using ArchiMetrics.UI.ViewModel;
	using Autofac;
	using Ionic.Zip;
	using Microsoft.CodeAnalysis;
	using NHunspell;

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
			PrepNativeAssemblies();
			Schedulers.Dispatcher = new DispatcherScheduler(Dispatcher);
			Schedulers.Taskpool = TaskPoolScheduler.Default;
			var container = BuildContainer();
			var loader = new ModernContentLoader(container);
			Resources.Add("Loader", loader);
			base.OnStartup(e);
		}

		private static void PrepNativeAssemblies()
		{
			var tempFolder = Path.Combine(Path.GetTempPath(), "ArchiMetrics");
			if (!Directory.Exists(tempFolder))
			{
				Directory.CreateDirectory(tempFolder);
			}

			var x86FileName = Path.Combine(tempFolder, "HUnspellx86.dll");
			var executingAssembly = Assembly.GetExecutingAssembly();
			if (!File.Exists(x86FileName))
			{
				using (var x86 = executingAssembly.GetManifestResourceStream("ArchiMetrics.UI.Native.Hunspellx86.dll"))
				{
					using (var x86File = File.Create(x86FileName))
					{
						x86.CopyTo(x86File);
					}
				}
			}

			var x64FileName = Path.Combine(tempFolder, "HUnspellx64.dll");
			if (!File.Exists(x64FileName))
			{
				using (var x64 = executingAssembly.GetManifestResourceStream("ArchiMetrics.UI.Native.Hunspellx64.dll"))
				{
					using (var x64File = File.Create(x64FileName))
					{
						x64.CopyTo(x64File);
					}
				}
			}

			Hunspell.NativeDllPath = tempFolder;
		}

		private static IContainer BuildContainer()
		{
			var builder = new ContainerBuilder();

			var defaultDictionary = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArchiMetrics.UI.Dictionaries.dict-en.oxt");
			using (var dictFile = ZipFile.Read(defaultDictionary))
			{
				var affStream = new MemoryStream();
				var dicStream = new MemoryStream();
				dictFile.First(z => z.FileName == "en_US.aff").Extract(affStream);
				dictFile.First(z => z.FileName == "en_US.dic").Extract(dicStream);
				builder.RegisterInstance(new Hunspell(affStream.ToArray(), dicStream.ToArray()));
				affStream.Dispose();
				dicStream.Dispose();
			}

			var evaluationTypes = from type in AllRules.GetRules()
								  where typeof(ICodeEvaluation).IsAssignableFrom(type)
								  select type;

			foreach (var type in evaluationTypes)
			{
				builder.RegisterType(type).As<IEvaluation>();
			}

			builder.RegisterType<EventAggregator>().AsSelf().As<IObservable<IMessage>>().SingleInstance();
			builder.RegisterType<SpellChecker>().As<ISpellChecker>();
			builder.RegisterType<ModelEdgeItemFactory>().As<IAsyncFactory<IEnumerable<IModelNode>, IEnumerable<ModelEdgeItem>>>();
			builder.RegisterType<KnownPatterns>().As<IKnownPatterns>().As<ICollection<Regex>>().SingleInstance();
			builder.RegisterType<CodeMetricsCalculator>().As<ICodeMetricsCalculator>();
			builder.RegisterType<ProjectMetricsCalculator>().As<ProjectMetricsCalculator>();
			builder.RegisterType<NodeReviewer>().As<INodeInspector>();
			builder.RegisterType<MetricsRepository>().As<IProjectMetricsRepository>().SingleInstance();
			builder.RegisterType<SolutionProvider>().As<IProvider<string, Solution>>().SingleInstance();
			builder.RegisterType<CodeErrorRepository>().As<ICodeErrorRepository>().As<IResetable>().SingleInstance();
			builder.RegisterType<SolutionVertexRepository>().As<IVertexRepository>().SingleInstance();
			builder.RegisterType<VertexTransformProvider>().As<IProvider<string, ObservableCollection<TransformRule>>>().SingleInstance();
			builder.RegisterType<VertexViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<CodeErrorGraphViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<CodeReviewViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<GraphViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<StructureRulesViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<TestErrorGraphViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<SettingsViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<MemberMetricsDataGridViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<TypeMetricsDataGridViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<NamespaceMetricsDataGridViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<ProjectMetricsDataGridViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<MetricsChartViewModel>().As<ViewModelBase>().AsSelf().SingleInstance();
			builder.RegisterType<AvailableRules>().As<IAvailableRules>().SingleInstance();
			builder.RegisterType<ModelValidator>().As<IModelValidator>();
			builder.RegisterType<SyntaxTransformer>().As<ISyntaxTransformer>();
			builder.RegisterType<AppContext>().As<IAppContext>().SingleInstance();
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
