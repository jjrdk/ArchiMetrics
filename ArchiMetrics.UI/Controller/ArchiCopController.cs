// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchiCopController.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ArchiMetricsController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.Controller
{
	using System.Linq;

	using ArchiCop.UI.MvvmFoundation;

	using ArchiMetrics.Analysis;

	using ArchiCop.UI.Properties;
	using ArchiCop.UI.ViewModel;

	using ArchiMetrics.Common;

	public class ArchiMetricsController
	{
		// private readonly IBuildItemRepository _buildItemRepository;
		private readonly ICodeErrorRepository _codeErrorRepository;
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly IEdgeItemsRepository _dependencyItemRepository;
		private readonly IEdgeTransformer _filter;
		private readonly IRequirementTestAnalyzer _requirementTestAnalyzer;
		private readonly IVertexRuleDefinition _ruleDefinition;
		private readonly IShell _shell;

		public ArchiMetricsController(
			IShell shell, 
			ICodeErrorRepository codeErrorRepository, 
			IRequirementTestAnalyzer requirementTestAnalyzer, 
			ISolutionEdgeItemsRepositoryConfig config, 
			IEdgeItemsRepository dependencyItemRepository, 
			IEdgeTransformer filter, 
			IVertexRuleDefinition ruleDefinition)
		{
			this._shell = shell;
			this._codeErrorRepository = codeErrorRepository;
			this._requirementTestAnalyzer = requirementTestAnalyzer;
			this._config = config;

			// _buildItemRepository = buildItemRepository;
			this._dependencyItemRepository = dependencyItemRepository;
			this._filter = filter;
			this._ruleDefinition = ruleDefinition;

			// _shell.Commands.Add(
			// 	new CommandViewModel(
			// 		Strings.MainWindowViewModel_Command_ViewBuilds,
			// 		new RelayCommand<object>(param => ShowBuilds())));
			this._shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewDependencies, 
					new RelayCommand<object>(param => this.ShowDependencies())));

			this._shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewDependencyGraph, 
					new RelayCommand<object>(param => this.ShowDependencyGraph())));

			this._shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewRequirementsGraph, 
					new RelayCommand<object>(param => this.ShowRequirementsGraph())));

			this._shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewCircularReferences, 
					new RelayCommand<object>(param => this.ShowCircularReferences())));

			this._shell.Commands.Add(
				new CommandViewModel(
					Strings.CodeReviewViewModel_DisplayName, 
					new RelayCommand<object>(param => this.ShowCodeErrors())));

			this._shell.Commands.Add(
				new CommandViewModel(
					Strings.CodeErrorGraphViewModel_DisplayName, 
					new RelayCommand<object>(param => this.ShowCodeErrorGraph())));

			this._shell.Commands.Add(
				new CommandViewModel(
					Strings.TestErrorGraphViewModel_DisplayName, 
					new RelayCommand<object>(param => this.ShowTestErrorGraph())));
		}

		private void ShowCodeErrorGraph()
		{
			var workspace = this._shell.Workspaces.OfType<CodeErrorGraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new CodeErrorGraphViewModel(this._codeErrorRepository);
				this._shell.Workspaces.Add(workspace);
			}

			this._shell.SetActiveWorkspace(workspace);
		}

		private void ShowTestErrorGraph()
		{
			var workspace = this._shell.Workspaces.OfType<TestErrorGraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new TestErrorGraphViewModel(this._codeErrorRepository);
				this._shell.Workspaces.Add(workspace);
			}

			this._shell.SetActiveWorkspace(workspace);
		}

		private void ShowCodeErrors()
		{
			var workspace = this._shell.Workspaces.OfType<CodeReviewViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new CodeReviewViewModel(this._codeErrorRepository);
				this._shell.Workspaces.Add(workspace);
			}

			this._shell.SetActiveWorkspace(workspace);
		}

		private void ShowDependencyGraph()
		{
			var workspace = this._shell.Workspaces.OfType<GraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new GraphViewModel(this._dependencyItemRepository, this._filter);
				this._shell.Workspaces.Add(workspace);
			}

			this._shell.SetActiveWorkspace(workspace);
		}

		private void ShowRequirementsGraph()
		{
			var workspace = this._shell.Workspaces.OfType<RequirementGraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new RequirementGraphViewModel(this._requirementTestAnalyzer, this._config, this._filter);
				this._shell.Workspaces.Add(workspace);
			}

			this._shell.SetActiveWorkspace(workspace);
		}

		private void ShowCircularReferences()
		{
			var workspace = this._shell.Workspaces.OfType<CircularReferenceViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new CircularReferenceViewModel(this._dependencyItemRepository, this._filter, this._ruleDefinition);
				this._shell.Workspaces.Add(workspace);
			}

			this._shell.SetActiveWorkspace(workspace);
		}

		private void ShowDependencies()
		{
			var workspace = this._shell.Workspaces.OfType<EdgesViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new EdgesViewModel(this._dependencyItemRepository, this._filter, this._ruleDefinition);
				this._shell.Workspaces.Add(workspace);
			}

			this._shell.SetActiveWorkspace(workspace);
		}

		// private void ShowBuilds()
		// {
		// 	var workspace =
		// 		_shell.Workspaces.FirstOrDefault(vm => vm is BuildItemsViewModel)
		// 		as BuildItemsViewModel;

		// 	if (workspace == null)
		// 	{
		// 		workspace = new BuildItemsViewModel(_buildItemRepository);
		// 		_shell.Workspaces.Add(workspace);
		// 	}

		// 	_shell.SetActiveWorkspace(workspace);
		// }
	}
}
