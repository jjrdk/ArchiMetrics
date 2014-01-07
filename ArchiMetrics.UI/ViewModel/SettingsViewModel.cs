// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SettingsViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Reactive;
	using System.Reactive.Concurrency;
	using System.Reactive.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.DataAccess;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.Support.Messages;

	internal class SettingsViewModel : ViewModelBase
	{
		private readonly IDisposable _changeSubscription;
		private readonly IKnownPatterns _patterns;
		private readonly IAppContext _config;
		private readonly IEnumerable<IResetable> _resetables;
		private readonly EventAggregator _eventAggregator;
		private readonly IDisposable _newPatternSubscription;

		public SettingsViewModel(
			IAvailableRules availableRules,
			ICollection<Regex> knownPatterns,
			IKnownPatterns patterns,
			IAppContext config,
			IEnumerable<IResetable> resetables,
			EventAggregator eventAggregator)
			: base(config)
		{
			AvailableRules = availableRules;
			KnownPatterns = knownPatterns;
			_patterns = patterns;
			_config = config;
			_resetables = resetables;
			_eventAggregator = eventAggregator;
			_changeSubscription = Observable
				.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
					h => _config.PropertyChanged += h, 
					h => _config.PropertyChanged -= h)
				.Select(x => x.EventArgs)
				.ObserveOn(Schedulers.Taskpool)
				.Subscribe(RaisePropertyChanged);
			_newPatternSubscription = Observable
				.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
					h => ((INotifyCollectionChanged)KnownPatterns).CollectionChanged += h,
					h => ((INotifyCollectionChanged)KnownPatterns).CollectionChanged -= h)
				.Throttle(TimeSpan.FromSeconds(3))
				.Subscribe(ResetData);
			AddSpellingCommand = new DelegateCommand(o => !string.IsNullOrWhiteSpace(o as string), AddSpelling);
			DeleteSpellingCommand = new DelegateCommand(o => ((IList)o).Count > 0, DeleteSelected);
		}

		public string Path
		{
			get { return _config.Path; }
			set { _config.Path = value; }
		}

		public int MaxNamespaceDepth
		{
			get { return _config.MaxNamespaceDepth; }
			set { _config.MaxNamespaceDepth = value; }
		}

		public bool IncludeCodeReview
		{
			get { return _config.IncludeCodeReview; }
			set { _config.IncludeCodeReview = value; }
		}

		public IAvailableRules AvailableRules { get; private set; }

		public ICollection<Regex> KnownPatterns { get; private set; }

		public DelegateCommand AddSpellingCommand { get; private set; }

		public DelegateCommand DeleteSpellingCommand { get; private set; }

		public void ImportPatterns(IEnumerable<string> patterns)
		{
			_patterns.Clear();
			_patterns.Add(patterns.WhereNotNullOrWhitespace().ToArray());
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_changeSubscription.Dispose();
				_newPatternSubscription.Dispose();
			}

			base.Dispose(isDisposing);
		}

		private void ResetData(EventPattern<NotifyCollectionChangedEventArgs> x)
		{
			foreach (var resetable in _resetables)
			{
				resetable.Reset();
			}

			_eventAggregator.Publish(new CodeReviewResetMessage());
		}

		private void DeleteSelected(object obj)
		{
			var input = (IList)obj;
			var items = input.OfType<Regex>().ToArray();
			foreach (var item in items)
			{
				KnownPatterns.Remove(item);
			}
		}

		private void AddSpelling(object obj)
		{
			var input = obj as string;
			if (input != null)
			{
				KnownPatterns.Add(new Regex(input, RegexOptions.Compiled));
			}
		}
	}
}
