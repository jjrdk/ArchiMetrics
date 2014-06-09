// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsSpelling.xaml.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for SettingsSpelling.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View.Tabs
{
	using System;
	using System.IO;
	using System.Reactive;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Windows;
	using System.Windows.Controls;
	using ArchiMetrics.Common;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;
	using Microsoft.Win32;

	/// <summary>
	/// Interaction logic for SettingsSpelling.xaml.
	/// </summary>
	[DataContext(typeof(SettingsViewModel))]
	public partial class SettingsSpelling : UserControl, IDisposable
	{
		private const string FileFilter = "Spelling files (*.spelling)|*.spelling|Analysis Dictionary files (*.xml)|*.xml|All Files (*.*)|*.*";
		private readonly Subject<Unit> _subject = new Subject<Unit>();
		private readonly IDisposable _subscription;

		public SettingsSpelling()
		{
			InitializeComponent();
			_subscription = _subject
				.Throttle(TimeSpan.FromSeconds(1))
				.ObserveOn(Schedulers.Dispatcher)
				.Subscribe(OnNextSelectionChange);
		}

		~SettingsSpelling()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_subscription.DisposeNotNull();
				_subject.DisposeNotNull();
			}
		}

		private void OnNextSelectionChange(Unit x)
		{
			((SettingsViewModel)DataContext).DeleteSpellingCommand.UpdateCanExecute();
		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Filter = FileFilter
			};
			if (dialog.ShowDialog() == true)
			{
				var context = DataContext as SettingsViewModel;
				var loader = new SpellingLoader();
				var words = loader.Load(dialog.FileName);

				context.ImportPatterns(words);
			}
		}

		private void OnSave(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				Filter = FileFilter,
				AddExtension = true
			};
			if (dialog.ShowDialog() == true)
			{
				using (var stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write))
				{
					using (var writer = new StreamWriter(stream))
					{
						var context = DataContext as SettingsViewModel;
						foreach (var pattern in context.KnownPatterns)
						{
							writer.WriteLine(pattern);
						}
					}
				}
			}
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			var context = DataContext as SettingsViewModel;
			context.AddSpellingCommand.UpdateCanExecute();
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_subject.OnNext(Unit.Default);
		}
	}
}
