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

namespace ArchiMeter.UI.ViewModel
{
	using System;
	using System.ComponentModel;
	using System.Threading;

	using ArchiMeter.Common;

	/// <summary>
	/// The ViewModel for the application's main window.
	/// </summary>
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;

		public MainWindowViewModel(ISolutionEdgeItemsRepositoryConfig config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}

			this._config = config;
			this._config.PropertyChanged += this.ConfigPropertyChanged;
		}

		public string Path
		{
			get
			{
				return this._config.Path;
			}

			set
			{
				if (this._config.Path != value)
				{
					this._config.Path = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public bool IncludeCodeReview
		{
			get
			{
				return this._config.IncludeCodeReview;
			}

			set
			{
				if (this._config.IncludeCodeReview != value)
				{
					this._config.IncludeCodeReview = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public EdgeSource Source
		{
			get
			{
				return this._config.Source;
			}

			set
			{
				if (this._config.Source != value)
				{
					this._config.Source = value;
					this.RaisePropertyChanged();
				}
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
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