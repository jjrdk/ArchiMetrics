namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Reactive.Linq;
	using System.Runtime.CompilerServices;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Structure;

	public class AppContext : IAppContext
	{
		private readonly IAvailableRules _availableRules;
		private readonly IDisposable _subscription;
		private int _maxNamespaceDepth = 2;
		private string _path = string.Empty;
		private string _rulesSource;

		public AppContext(IAvailableRules availableRules)
		{
			_rulesSource = string.Empty;
			_availableRules = availableRules;
			_subscription = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
				h => _availableRules.CollectionChanged += h,
				h => _availableRules.CollectionChanged -= h)
				.Throttle(TimeSpan.FromSeconds(3))
				.Subscribe(x => OnPropertyChanged(string.Empty));
			CutOff = TimeSpan.FromDays(7);
		}

		~AppContext()
		{
			Dispose(false);
		}

		public TimeSpan CutOff { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public string Path
		{
			get
			{
				return _path;
			}

			set
			{
				if (_path != (value ?? string.Empty))
				{
					_path = value ?? string.Empty;
					OnPropertyChanged();
				}
			}
		}

		public string RulesSource
		{
			get
			{
				return _rulesSource;
			}

			set
			{
				if (_rulesSource != value)
				{
					_rulesSource = value;
					OnPropertyChanged();
				}
			}
		}

		public int MaxNamespaceDepth
		{
			get
			{
				return _maxNamespaceDepth;
			}

			set
			{
				if (!_maxNamespaceDepth.Equals(value))
				{
					_maxNamespaceDepth = value;
					OnPropertyChanged();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_subscription.Dispose();
			}
		}
	}
}
