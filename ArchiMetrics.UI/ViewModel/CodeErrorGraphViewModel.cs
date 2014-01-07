// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrorGraphViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeErrorGraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ArchiMetrics.CodeReview.Rules;

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;

	internal class CodeErrorGraphViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly ICodeErrorRepository _repository;
		private CancellationTokenSource _tokenSource;
		private IList<KeyValuePair<string, int>> _errorsByTitle;
		private IList<KeyValuePair<string, int>> _errorsByNamespace;
		private IList<KeyValuePair<string, int>> _errorsByQualityAttribute;

		public CodeErrorGraphViewModel(ICodeErrorRepository repository, IAppContext config)
			: base(config)
		{
			_repository = repository;
			_config = config;
#pragma warning disable 4014
			UpdateInternal(true);
#pragma warning restore 4014
		}

		public IList<KeyValuePair<string, int>> ErrorsByTitle
		{
			get
			{
				return _errorsByTitle;
			}

			private set
			{
				_errorsByTitle = value;
				RaisePropertyChanged();
			}
		}

		public IList<KeyValuePair<string, int>> ErrorsByNamespace
		{
			get
			{
				return _errorsByNamespace;
			}

			private set
			{
				_errorsByNamespace = value;
				RaisePropertyChanged();
			}
		}

		public IList<KeyValuePair<string, int>> ErrorsByQualityAttribute
		{
			get
			{
				return _errorsByQualityAttribute;
			}

			private set
			{
				_errorsByQualityAttribute = value;
				RaisePropertyChanged();
			}
		}

		protected async override void Update(bool forceUpdate)
		{
			await UpdateInternal(forceUpdate);
		}

		protected override void Dispose(bool isDisposing)
		{
			base.Dispose(isDisposing);
			if (isDisposing)
			{
				_repository.Dispose();
			}
		}

		private async Task UpdateInternal(bool forceUpdate)
		{
			IsLoading = true;
			ErrorsByTitle = new List<KeyValuePair<string, int>>();
			if (_tokenSource != null)
			{
				_tokenSource.Cancel(false);
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			base.Update(forceUpdate);
			if (forceUpdate)
			{
				var result = (await _repository.GetErrors(_config.Path, _tokenSource.Token)).ToArray();
				if (!_tokenSource.IsCancellationRequested)
				{
					await DisplayErrors(result);
				}
			}
		}

		private async Task DisplayErrors(EvaluationResult[] results)
		{
			IsLoading = true;

			var titleTask = DisplayErrorsByTitle(results);
			var qualityAttributeTask = DisplayErrorsByQualityAttribute(results);
			var namespaceTask = DisplayErrorsByNamespace(results);

			await Task.WhenAll(titleTask, qualityAttributeTask, namespaceTask);
			IsLoading = false;
		}

		private Task DisplayErrorsByTitle(IEnumerable<EvaluationResult> result)
		{
			return Task.Factory.StartNew(() =>
			{
				var results = result
					.GroupBy(x => x.Title)
					.Where(x => !string.IsNullOrWhiteSpace(x.Key))
					.Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
					.OrderBy(x => x.Key)
					.ToList();
				ErrorsByTitle = results;
			});
		}

		private Task DisplayErrorsByNamespace(IEnumerable<EvaluationResult> result)
		{
			return Task.Factory.StartNew(() =>
			{
				var results = result
					.GroupBy(x => string.Join(".", x.Namespace.Split('.').Take(_config.MaxNamespaceDepth)))
					.Where(x => !string.IsNullOrWhiteSpace(x.Key))
					.Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
					.OrderBy(x => x.Key)
					.ToList();
				ErrorsByNamespace = results;
			});
		}

		private Task DisplayErrorsByQualityAttribute(IEnumerable<EvaluationResult> result)
		{
			return Task.Factory.StartNew(() =>
			{
				var qualityAtttributeItems = Enum.GetValues(typeof(QualityAttribute))
					.OfType<Enum>()
					.SelectMany(e => result.Where(x => x.QualityAttribute.HasFlag(e)).Select(r => e))
					.GroupBy(x => x)
					.Select(x => new KeyValuePair<string, int>(x.Key.ToString().ToTitleCase(), x.Count()))
					.OrderBy(x => x.Key)
					.ToList();
				ErrorsByQualityAttribute = qualityAtttributeItems;
			});
		}
	}
}
