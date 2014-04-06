namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Runtime.CompilerServices;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;

	internal class AvailableRules : IAvailableRules
	{
		private readonly List<AvailableRule> _innerList;
		private readonly IEnumerable<IDisposable> _subscriptions;

		public AvailableRules(IEnumerable<IEvaluation> evaluations)
		{
			_innerList = (from evaluation in evaluations
						  orderby evaluation.Title
						  select new AvailableRule(evaluation))
				.ToList();
			_subscriptions = _innerList.Select(
				x => Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
				h => x.PropertyChanged += h,
				h => x.PropertyChanged -= h)
				.Throttle(TimeSpan.FromSeconds(1))
				.Subscribe(y => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))))
				.ToArray();
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public IEnumerable<IAvailability> Availabilities
		{
			get { return _innerList; }
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public IEnumerator<IEvaluation> GetEnumerator()
		{
			return _innerList
				.Where(x => x.IsAvailable)
				.Select(x => x.Rule)
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			var handler = CollectionChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				foreach (var subscription in _subscriptions)
				{
					subscription.Dispose();
				}
			}
		}

		private class AvailableRule : IAvailability, INotifyPropertyChanged
		{
			private bool _isAvailable;
			private IEvaluation _rule;

			public AvailableRule(IEvaluation rule)
			{
				_isAvailable = true;
				_rule = rule;
			}

			public IEvaluation Rule
			{
				get
				{
					return _rule;
				}

				private set
				{
					if (!ReferenceEquals(_rule, value))
					{
						_rule = value;
						OnPropertyChanged();
					}
				}
			}

			public bool IsAvailable
			{
				get
				{
					return _isAvailable;
				}

				set
				{
					_isAvailable = value;
					OnPropertyChanged();
				}
			}

			public string Title
			{
				get
				{
					return _rule.Title;
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;

			public override string ToString()
			{
				return Rule.Title;
			}

			protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				var handler = PropertyChanged;
				if (handler != null)
				{
					handler(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}
	}
}
