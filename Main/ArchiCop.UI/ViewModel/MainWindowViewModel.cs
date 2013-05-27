// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   The ViewModel for the application's main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Data;
	using System.Windows.Input;

	using ArchiCop.UI.MvvmFoundation;

	using ArchiMeter.Common;

	/// <summary>
	/// The ViewModel for the application's main window.
	/// </summary>
	public class MainWindowViewModel : WorkspaceViewModel, IShell
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly object _syncToken = new object();
		private ObservableCollection<CommandViewModel> _commands;
		private CancellationTokenSource _tokenSource;
		private ObservableCollection<WorkspaceViewModel> _workspaces;

		public MainWindowViewModel(ISolutionEdgeItemsRepositoryConfig config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}

			_config = config;
			_config.PropertyChanged += ConfigPropertyChanged;
		}

		public string Path
		{
			get
			{
				return _config.Path;
			}

			set
			{
				if (_config.Path != value)
				{
					_config.Path = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool IncludeCodeReview
		{
			get
			{
				return _config.IncludeCodeReview;
			}

			set
			{
				if (_config.IncludeCodeReview != value)
				{
					_config.IncludeCodeReview = value;
					RaisePropertyChanged();
				}
			}
		}

		public EdgeSource Source
		{
			get
			{
				return _config.Source;
			}

			set
			{
				if (_config.Source != value)
				{
					_config.Source = value;
					RaisePropertyChanged();
				}
			}
		}

		public ICommand UpdateCommand
		{
			get { return new RelayCommand(() => UpdateAllWorkspaces(false)); }
		}

		/// <summary>
		/// Returns a list of commands 
		/// that the UI can display and execute.
		/// </summary>
		public ObservableCollection<CommandViewModel> Commands
		{
			get { return _commands ?? (_commands = new ObservableCollection<CommandViewModel>(CreateCommands())); }
		}

		/// <summary>
		/// Returns the collection of available workspaces to display.
		/// A 'workspace' is a ViewModel that can request to be closed.
		/// </summary>
		public ObservableCollection<WorkspaceViewModel> Workspaces
		{
			get
			{
				if (_workspaces == null)
				{
					_workspaces = new ObservableCollection<WorkspaceViewModel>();
					BindingOperations.EnableCollectionSynchronization(_workspaces, _syncToken);
					_workspaces.CollectionChanged += OnWorkspacesChanged;
				}

				return _workspaces;
			}
		}

		public void SetActiveWorkspace(WorkspaceViewModel workspace)
		{
			var collectionView = CollectionViewSource.GetDefaultView(Workspaces);
			if (collectionView != null)
			{
				collectionView.MoveCurrentTo(workspace);
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (_tokenSource != null)
				{
					_tokenSource.Cancel();
					_tokenSource.Dispose();
				}

				_config.PropertyChanged -= ConfigPropertyChanged;
				_commands.Clear();
				CloseWorkspaces();
			}

			base.Dispose(isDisposing);
		}

		private void CloseWorkspaces()
		{
			if (_workspaces != null)
			{
				BindingOperations.DisableCollectionSynchronization(_workspaces);
				foreach (var workspace in _workspaces.ToArray())
				{
					workspace.CloseCommand.Execute(null);
				}

				_workspaces.Clear();
			}
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Path":
					CloseWorkspaces();

					break;
				default:
					Task.Factory.StartNew(() => UpdateAllWorkspaces(true));
					break;
			}
		}

		private void UpdateAllWorkspaces(bool forceUpdate)
		{
			if (_tokenSource != null)
			{
				_tokenSource.Cancel();
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			var token = _tokenSource.Token;

			foreach (var workspace in Workspaces.ToArray())
			{
				var workspace1 = workspace;
				Task.Factory.StartNew(() => workspace1.Update(forceUpdate), token);
			}
		}

		private List<CommandViewModel> CreateCommands()
		{
			return new List<CommandViewModel>();
		}

		private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null && e.NewItems.Count != 0)
			{
				foreach (WorkspaceViewModel workspace in e.NewItems)
				{
					workspace.RequestClose += OnWorkspaceRequestClose;
				}
			}

			if (e.OldItems != null && e.OldItems.Count != 0)
			{
				foreach (WorkspaceViewModel workspace in e.OldItems)
				{
					workspace.RequestClose -= OnWorkspaceRequestClose;
				}
			}
		}

		private void OnWorkspaceRequestClose(object sender, EventArgs e)
		{
			var workspace = (WorkspaceViewModel)sender;
			Workspaces.Remove(workspace);
			workspace.Dispose();
		}
	}
}