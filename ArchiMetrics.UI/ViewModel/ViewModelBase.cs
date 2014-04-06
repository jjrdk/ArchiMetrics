namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics.CodeAnalysis;
	using System.Reactive.Linq;
	using System.Runtime.CompilerServices;
	using ArchiMetrics.Common.Structure;

	/// <summary>
	/// Base class for all ViewModel classes in the application.
	/// It provides support for property change notifications 
	/// and has a DisplayName property.  This class is abstract.
	/// </summary>
	internal abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		private readonly IAppContext _config;
		private IDisposable _changeSubscription;
		private bool _isLoading;

		protected ViewModelBase(IAppContext config)
		{
			_config = config;
			_changeSubscription = Observable
				.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
					h => _config.PropertyChanged += h, 
					h => _config.PropertyChanged -= h)
				.SubscribeOn(Schedulers.Taskpool)
				.ObserveOn(Schedulers.Taskpool)
				.Subscribe(x => Update(true));
		}

		~ViewModelBase()
		{
			Dispose(false);
		}

		public bool IsLoading
		{
			get
			{
				return _isLoading;
			}

			set
			{
				if (_isLoading != value)
				{
					_isLoading = value;
					RaisePropertyChanged();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void Dispose(bool isDisposing)
		{
			if (_changeSubscription != null)
			{
				_changeSubscription.Dispose();
				_changeSubscription = null;
			}

			if (isDisposing)
			{
			}
		}

		protected virtual void Update(bool forceUpdate)
		{
		}

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This is the event invocation.")]
		protected void RaisePropertyChanged([CallerMemberName]string propertyName = "")
		{
			var e = new PropertyChangedEventArgs(propertyName);
			RaisePropertyChanged(e);
		}

		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Event invocation method.")]
		protected void RaisePropertyChanged(PropertyChangedEventArgs args)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, args);
			}
		}
	}
}
