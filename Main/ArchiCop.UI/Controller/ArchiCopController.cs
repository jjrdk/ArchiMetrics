// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArchiCopController.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ArchiMeterController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.Controller
{
	using System.Linq;
	using ArchiMeter.Analysis;
	using ArchiMeter.Common;
	using MvvmFoundation;
	using Properties;
	using ViewModel;

	public class ArchiMeterController
	{
		// private readonly IBuildItemRepository _buildItemRepository;
		private readonly ICodeErrorRepository _codeErrorRepository;
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly IEdgeItemsRepository _dependencyItemRepository;
		private readonly IEdgeTransformer _filter;
		private readonly IRequirementTestAnalyzer _requirementTestAnalyzer;
		private readonly IVertexRuleDefinition _ruleDefinition;
		private readonly IShell _shell;

		public ArchiMeterController(
			IShell shell, 
			ICodeErrorRepository codeErrorRepository, 
			IRequirementTestAnalyzer requirementTestAnalyzer, 
			ISolutionEdgeItemsRepositoryConfig config, 
			IEdgeItemsRepository dependencyItemRepository, 
			IEdgeTransformer filter, 
			IVertexRuleDefinition ruleDefinition)
		{
			_shell = shell;
			_codeErrorRepository = codeErrorRepository;
			_requirementTestAnalyzer = requirementTestAnalyzer;
			_config = config;

			// _buildItemRepository = buildItemRepository;
			_dependencyItemRepository = dependencyItemRepository;
			_filter = filter;
			_ruleDefinition = ruleDefinition;

			// _shell.Commands.Add(
			// 	new CommandViewModel(
			// 		Strings.MainWindowViewModel_Command_ViewBuilds,
			// 		new RelayCommand<object>(param => ShowBuilds())));
			_shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewDependencies, 
					new RelayCommand<object>(param => ShowDependencies())));

			_shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewDependencyGraph, 
					new RelayCommand<object>(param => ShowDependencyGraph())));

			_shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewRequirementsGraph, 
					new RelayCommand<object>(param => ShowRequirementsGraph())));

			_shell.Commands.Add(
				new CommandViewModel(
					Strings.MainWindowViewModel_Command_ViewCircularReferences, 
					new RelayCommand<object>(param => ShowCircularReferences())));

			_shell.Commands.Add(
				new CommandViewModel(
					Strings.CodeReviewViewModel_DisplayName, 
					new RelayCommand<object>(param => ShowCodeErrors())));

			_shell.Commands.Add(
				new CommandViewModel(
					Strings.CodeErrorGraphViewModel_DisplayName, 
					new RelayCommand<object>(param => ShowCodeErrorGraph())));

			_shell.Commands.Add(
				new CommandViewModel(
					Strings.TestErrorGraphViewModel_DisplayName, 
					new RelayCommand<object>(param => ShowTestErrorGraph())));
		}

		private void ShowCodeErrorGraph()
		{
			var workspace = _shell.Workspaces.OfType<CodeErrorGraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new CodeErrorGraphViewModel(_codeErrorRepository);
				_shell.Workspaces.Add(workspace);
			}

			_shell.SetActiveWorkspace(workspace);
		}

		private void ShowTestErrorGraph()
		{
			var workspace = _shell.Workspaces.OfType<TestErrorGraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new TestErrorGraphViewModel(_codeErrorRepository);
				_shell.Workspaces.Add(workspace);
			}

			_shell.SetActiveWorkspace(workspace);
		}

		private void ShowCodeErrors()
		{
			var workspace = _shell.Workspaces.OfType<CodeReviewViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new CodeReviewViewModel(_codeErrorRepository);
				_shell.Workspaces.Add(workspace);
			}

			_shell.SetActiveWorkspace(workspace);
		}

		private void ShowDependencyGraph()
		{
			var workspace = _shell.Workspaces.OfType<GraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new GraphViewModel(_dependencyItemRepository, _filter);
				_shell.Workspaces.Add(workspace);
			}

			_shell.SetActiveWorkspace(workspace);
		}

		private void ShowRequirementsGraph()
		{
			var workspace = _shell.Workspaces.OfType<RequirementGraphViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new RequirementGraphViewModel(_requirementTestAnalyzer, _config, _filter);
				_shell.Workspaces.Add(workspace);
			}

			_shell.SetActiveWorkspace(workspace);
		}

		private void ShowCircularReferences()
		{
			var workspace = _shell.Workspaces.OfType<CircularReferenceViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new CircularReferenceViewModel(_dependencyItemRepository, _filter, _ruleDefinition);
				_shell.Workspaces.Add(workspace);
			}

			_shell.SetActiveWorkspace(workspace);
		}

		private void ShowDependencies()
		{
			var workspace = _shell.Workspaces.OfType<EdgesViewModel>().FirstOrDefault();

			if (workspace == null)
			{
				workspace = new EdgesViewModel(_dependencyItemRepository, _filter, _ruleDefinition);
				_shell.Workspaces.Add(workspace);
			}

			_shell.SetActiveWorkspace(workspace);
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