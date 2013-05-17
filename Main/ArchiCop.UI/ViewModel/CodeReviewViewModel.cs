// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeReviewViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeReviewViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.ViewModel
{
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Windows.Data;
	using ArchiMeter.Common;

	public class CodeReviewViewModel : WorkspaceViewModel
	{
		private readonly object _syncLock = new object();
		private int _brokenCode;
		private ObservableCollection<EvaluationResult> _codeErrors;
		private int _errorsShown;
		private int _filesWithErrors;

		public CodeReviewViewModel(ICodeErrorRepository repository)
		{
			IsLoading = true;
			CodeErrors = new ObservableCollection<EvaluationResult>();
			repository.GetErrorsAsync()
					  .ContinueWith(t =>
						  {
							  if (t.Exception != null)
							  {
								  var exception = t.Exception.InnerExceptions[0];

								  CodeErrors.Add(new EvaluationResult
													 {
														 Quality = CodeQuality.Broken, 
														 Comment = exception.Message, 
														 Snippet = exception.StackTrace
													 });
								  IsLoading = false;
								  return;
							  }

							  ErrorsShown = 0;
							  CodeErrors.Clear();

							  var results = t.Result.Where(x => x.Comment != "Multiple asserts found in test." || x.ErrorCount != 1).ToArray();
							  foreach (var result in results)
							  {
								  CodeErrors.Add(result);
							  }

							  ErrorsShown = CodeErrors.Count;
							  if (CodeErrors.Count == 0)
							  {
								  CodeErrors.Add(new EvaluationResult { Comment = "No Errors", Quality = CodeQuality.Good });
							  }

							  FilesWithErrors = results.GroupBy(x => x.FilePath).Select(x => x.Key).Count();
							  BrokenCode = (int)(results
													 .Where(x => x.Quality == CodeQuality.Broken || x.Quality == CodeQuality.Incompetent)
													 .Sum(x => (double)x.LinesOfCodeAffected)
												 + results.Where(x => x.Quality == CodeQuality.NeedsReEngineering)
														  .Sum(x => x.LinesOfCodeAffected * .5)
												 + results.Where(x => x.Quality == CodeQuality.NeedsReEngineering)
														  .Sum(x => x.LinesOfCodeAffected * .2));
							  IsLoading = false;
						  });
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
				if (value != _codeErrors)
				{
					if (_codeErrors != null)
					{
						BindingOperations.DisableCollectionSynchronization(_codeErrors);
					}

					_codeErrors = value;
					if (_codeErrors != null)
					{
						BindingOperations.EnableCollectionSynchronization(_codeErrors, _syncLock);
					}

					RaisePropertyChanged();
				}
			}
		}
	}
}