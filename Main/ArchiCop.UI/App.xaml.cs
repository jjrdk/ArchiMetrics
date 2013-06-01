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

namespace ArchiCop.UI
{
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Windows;
	using System.Windows.Markup;
	using ArchiMeter.Analysis;
	using ArchiMeter.CodeReview;
	using ArchiMeter.CodeReview.Metrics;
	using ArchiMeter.Common;
	using ArchiMeter.Common.Metrics;
	using ArchiMeter.Data.DataAccess;
	using Autofac;
	using Ionic.Zip;
	using NHunspell;
	using Roslyn.Services;
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
			var builder = new ContainerBuilder();

			// container.RegisterType<IBuildItemRepository, FakeBuildItemRepository>(
			// 	new ContainerControlledLifetimeManager());
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
			builder.RegisterType<SpellChecker>().As<ISpellChecker>();
			builder.RegisterType<KnownWordList>().As<IKnownWordList>();
			builder.RegisterType<ProjectMetricsCalculator>()
				   .As<IProjectMetricsCalculator>();
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
				.AsSelf();
			builder.RegisterType<CircularReferenceViewModel>()
				.AsSelf();
			builder.RegisterType<CodeErrorGraphViewModel>()
				.AsSelf();
			builder.RegisterType<CodeReviewViewModel>()
				.AsSelf();
			builder.RegisterType<GraphViewModel>()
				.AsSelf();
			builder.RegisterType<RequirementGraphViewModel>()
				.AsSelf();
			builder.RegisterType<TestErrorGraphViewModel>()
				.AsSelf();
			var container = builder.Build();
			var loader = new ModernContentLoader(container);
			Resources.Add("Loader", loader);
			base.OnStartup(e);
		}
	}
}