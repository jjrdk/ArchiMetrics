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

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;

	public class CodeErrorGraphViewModel : ViewModelBase
	{
		private readonly ICodeErrorRepository _repository;
		private readonly IAppContext _config;
		private CancellationTokenSource _tokenSource;
		private IList<KeyValuePair<string, int>> _errors;

		public CodeErrorGraphViewModel(ICodeErrorRepository repository, IAppContext config)
			: base(config)
		{
			_repository = repository;
			_config = config;
			UpdateInternal(true);
		}

		public IList<KeyValuePair<string, int>> Errors
		{
			get
			{
				return _errors;
			}

			private set
			{
				_errors = value;
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
			Errors = new List<KeyValuePair<string, int>>();
			if (_tokenSource != null)
			{
				_tokenSource.Cancel(false);
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			base.Update(forceUpdate);
			if (forceUpdate)
			{
				var result = await _repository.GetErrors(_config.Path, _tokenSource.Token);
				if (!_tokenSource.IsCancellationRequested)
				{
					DisplayErrors(result);
				}
			}
		}

		private void DisplayErrors(IEnumerable<EvaluationResult> result)
		{
			IsLoading = true;
			var results = result
				.GroupBy(x => x.Title)
				.Where(x => !string.IsNullOrWhiteSpace(x.Key))
				.Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
				.OrderBy(x => x.Key)
				.ToList();
			Errors = results;
			IsLoading = false;
		}
	}
}
