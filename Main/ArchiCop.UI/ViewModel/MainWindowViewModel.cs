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

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.ComponentModel;
	using System.Threading;

	using ArchiMeter.Common;

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
			_config.PropertyChanged += this.ConfigPropertyChanged;
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
					this.RaisePropertyChanged();
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
					this.RaisePropertyChanged();
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
					this.RaisePropertyChanged();
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

				_config.PropertyChanged -= this.ConfigPropertyChanged;
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