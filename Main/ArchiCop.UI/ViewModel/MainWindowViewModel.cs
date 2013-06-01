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
	using ArchiMeter.Common;
	using FirstFloor.ModernUI.Presentation;
	using RelayCommand = MvvmFoundation.RelayCommand;

	/// <summary>
	/// The ViewModel for the application's main window.
	/// </summary>
	public class MainWindowViewModel : WorkspaceViewModel
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly object _syncToken = new object();
		private CancellationTokenSource _tokenSource;

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
			}

			base.Dispose(isDisposing);
		}

		private void ConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Path":
					break;
				default:
					break;
			}
		}
	}
}