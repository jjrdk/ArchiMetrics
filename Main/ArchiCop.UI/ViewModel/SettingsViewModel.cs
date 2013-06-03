namespace ArchiMeter.UI.ViewModel
{
	using System;
	using System.Reactive.Linq;

	using ArchiMeter.Common;

	using System.ComponentModel;

	internal class SettingsViewModel : ViewModelBase
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly IDisposable _changeSubscription;

		public SettingsViewModel(ISolutionEdgeItemsRepositoryConfig config)
		{
			this._config = config;
			this._changeSubscription = Observable
				.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
					h => this._config.PropertyChanged += h,
					h => this._config.PropertyChanged -= h)
				.Select(x => x.EventArgs)
				.Subscribe(this.RaisePropertyChanged);
		}

		public string Path
		{
			get { return this._config.Path; }
			set { this._config.Path = value; }
		}

		public EdgeSource Source
		{
			get { return this._config.Source; }
			set { this._config.Source = value; }
		}

		public bool IncludeCodeReview
		{
			get { return this._config.IncludeCodeReview; }
			set { this._config.IncludeCodeReview = value; }
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				this._changeSubscription.Dispose();
			}

			base.Dispose(isDisposing);
		}
	}
}