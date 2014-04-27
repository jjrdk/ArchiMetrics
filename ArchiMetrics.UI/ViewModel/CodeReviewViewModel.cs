// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeReviewViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeReviewViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Reactive.Linq;
	using System.Threading;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;
	using ArchiMetrics.UI.Support.Messages;

	internal sealed class CodeReviewViewModel : ViewModelBase
	{
		private readonly IAppContext _config;
		private readonly ICodeErrorRepository _repository;
		private readonly IDisposable _subscription;
		private int _brokenCode;
		private ObservableCollection<EvaluationResult> _codeErrors;
		private int _errorsShown;
		private int _filesWithErrors;
		private CancellationTokenSource _tokenSource;

		public CodeReviewViewModel(ICodeErrorRepository repository, IAppContext config, IObservable<IMessage> eventAggregator)
			: base(config)
		{
			_subscription = eventAggregator.OfType<CodeReviewResetMessage>().Subscribe(x => Update(true));
			_repository = repository;
			_config = config;
			IsLoading = true;
			CodeErrors = new ObservableCollection<EvaluationResult>();
			Update(true);
		}

		public int BrokenCode
		{
			get
			{
				return _brokenCode;
			}

			set
			{
				if (_brokenCode != value)
				{
					_brokenCode = value;
					RaisePropertyChanged();
				}
			}
		}

		public int ErrorsShown
		{
			get
			{
				return _errorsShown;
			}

			set
			{
				if (_errorsShown != value)
				{
					_errorsShown = value;
					RaisePropertyChanged();
				}
			}
		}

		public int FilesWithErrors
		{
			get
			{
				return _filesWithErrors;
			}

			set
			{
				if (_filesWithErrors != value)
				{
					_filesWithErrors = value;
					RaisePropertyChanged();
				}
			}
		}

		public ObservableCollection<EvaluationResult> CodeErrors
		{
			get
			{
				return _codeErrors;
			}

			private set
			{
				_codeErrors = value;
				RaisePropertyChanged();
			}
		}

		protected async override void Update(bool forceUpdate)
		{
			IsLoading = true;
			if (_tokenSource != null)
			{
				_tokenSource.Cancel(false);
				_tokenSource.Dispose();
			}

			_tokenSource = new CancellationTokenSource();
			try
			{
				ErrorsShown = 0;

				var errors = await _repository.GetErrors(_config.Path, _tokenSource.Token).ConfigureAwait(false);
				var results = errors.OrderBy(x => x.Title).ToArray();
				var newErrors = new ObservableCollection<EvaluationResult>(results);

				if (newErrors.Count == 0)
				{
					var noerrors = new EvaluationResult
					{
						Title = "No Errors",
						Quality = CodeQuality.Good
					};
					newErrors.Add(noerrors);
				}

				FilesWithErrors = results.GroupBy(x => x.FilePath).Select(x => x.Key).Count();
				BrokenCode = (int)(results
					.Where(x => x.Quality == CodeQuality.Broken || x.Quality == CodeQuality.NeedsCleanup)
					.Sum(x => (double)x.LinesOfCodeAffected)
								   + results.Where(x => x.Quality == CodeQuality.NeedsReEngineering)
									   .Sum(x => x.LinesOfCodeAffected * .5)
								   + results.Where(x => x.Quality == CodeQuality.NeedsRefactoring)
									   .Sum(x => x.LinesOfCodeAffected * .2));

				CodeErrors = newErrors;
			}
			catch (Exception exception)
			{
				var result = new EvaluationResult
									   {
										   Quality = CodeQuality.Broken,
										   Title = exception.Message,
										   Snippet = exception.StackTrace
									   };
				var exceptionErrors = new ObservableCollection<EvaluationResult> { result };

				CodeErrors = exceptionErrors;
			}
			finally
			{
				IsLoading = false;
				ErrorsShown = CodeErrors.Count;
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_codeErrors.Clear();
				_tokenSource.DisposeNotNull();
				_subscription.Dispose();
			}

			base.Dispose(isDisposing);
		}
	}
}
