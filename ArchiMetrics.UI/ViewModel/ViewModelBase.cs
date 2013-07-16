// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Base class for all ViewModel classes in the application.
//   It provides support for property change notifications
//   and has a DisplayName property.  This class is abstract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.ComponentModel;
	using System.Diagnostics.CodeAnalysis;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using System.Runtime.CompilerServices;
	using Common;
	using Properties;

	/// <summary>
	/// Base class for all ViewModel classes in the application.
	/// It provides support for property change notifications 
	/// and has a DisplayName property.  This class is abstract.
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;

		private IDisposable _changeSubscription;
		private bool _isLoading;

		protected ViewModelBase(ISolutionEdgeItemsRepositoryConfig config)
		{
			_config = config;
			var type = GetType();
			DisplayName = Strings.ResourceManager.GetString(type.Name + "_DisplayName");
			_changeSubscription = Observable
				.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
					h => _config.PropertyChanged += h,
					h => _config.PropertyChanged -= h)
				.SubscribeOn(TaskPoolScheduler.Default)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => Update(true));
		}

		/// <summary>
		/// Raised when a property on this object has a new value.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

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

		/// <summary>
		/// Returns the user-friendly name of this object.
		/// Child classes can set this property to a new value,
		/// or override it to determine the value on-demand.
		/// </summary>
		public string DisplayName { get; protected set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

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
