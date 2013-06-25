// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the App type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI
{
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Windows;
	using System.Windows.Markup;
	using Analysis;
	using Analysis.Metrics;
	using Autofac;
	using CodeReview;
	using Common;
	using Common.Metrics;
	using Data.DataAccess;
	using Ionic.Zip;
	using NHunspell;
	using Roslyn.Services;
	using Support;
	using ViewModel;

	public partial class App : Application
	{
		static App()
		{
			// Ensure the current culture passed into bindings is the OS culture.
			// By default, WPF uses en-US as the culture, regardless of the system settings.
			FrameworkElement.LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
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
			var config = new SolutionEdgeItemsRepositoryConfig();
			builder.RegisterInstance<ISolutionEdgeItemsRepositoryConfig>(config);
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
			foreach (var type in typeof(IEvaluation).Assembly
												   .GetTypes()
												   .Where(t => typeof(IEvaluation).IsAssignableFrom(t))
												   .Where(t => !t.IsInterface && !t.IsAbstract))
			{
				builder.RegisterType(type)
					   .As<IEvaluation>();
			}
			builder.RegisterType<SpellChecker>().As<ISpellChecker>();
			builder.RegisterType<KnownWordList>().As<IKnownWordList>();
			builder.RegisterType<CodeMetricsCalculator>()
				   .As<ICodeMetricsCalculator>();
			builder.RegisterType<SolutionInspector>()
				   .As<INodeInspector>();
			var vertexRuleRepository = new FakeVertexRuleRepository();
			builder.RegisterInstance(new PathFilter(x => true))
				   .As<PathFilter>();
			builder.RegisterType<SolutionProvider>()
				   .As<IProvider<string, ISolution>>();
			builder.RegisterType<ProjectProvider>()
				   .As<IProvider<string, IProject>>();
			builder.RegisterType<CodeErrorRepository>()
				   .As<ICodeErrorRepository>();
			builder.RegisterType<AggregateEdgeItemsRepository>()
				   .As<IEdgeItemsRepository>();
			builder.RegisterInstance<IVertexRuleRepository>(vertexRuleRepository);
			builder.RegisterInstance<IVertexRuleDefinition>(vertexRuleRepository);
			builder.RegisterType<EdgeTransformer>()
				   .As<IEdgeTransformer>();
			builder.RegisterType<RequirementTestAnalyzer>()
				   .As<IRequirementTestAnalyzer>();
			builder.RegisterType<EdgesViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			builder.RegisterType<CircularReferenceViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			builder.RegisterType<CodeErrorGraphViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			builder.RegisterType<CodeReviewViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			builder.RegisterType<GraphViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			builder.RegisterType<RequirementGraphViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			builder.RegisterType<TestErrorGraphViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			builder.RegisterType<SettingsViewModel>()
				   .As<ViewModelBase>()
				   .AsSelf();
			var container = builder.Build();

			return container;
		}
	}
}