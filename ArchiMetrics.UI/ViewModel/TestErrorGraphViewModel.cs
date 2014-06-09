// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestErrorGraphViewModel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestErrorGraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Structure;

	internal class TestErrorGraphViewModel : ViewModelBase
	{
		private IList<KeyValuePair<int, int>> _errors;

		public TestErrorGraphViewModel(ICodeErrorRepository repository, IAppContext config)
			: base(config)
		{
			repository.GetErrors(config.Path, CancellationToken.None)
					  .ContinueWith(DisplayErrors);
		}

		public IList<KeyValuePair<int, int>> Errors
		{
			get
			{
				return _errors;
			}

			private set
			{
				if (!ReferenceEquals(_errors, value))
				{
					_errors = value;
					RaisePropertyChanged();
				}
			}
		}

		private void DisplayErrors(Task<IEnumerable<EvaluationResult>> task)
		{
			IsLoading = true;
			var results = task.Result.Where(x => x.Title == "Multiple Asserts in Test")
							  .GroupBy(x => x.ErrorCount)
							  .Where(x => x.Key != 1)
							  .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
							  .OrderBy(x => x.Key);
			Errors = new List<KeyValuePair<int, int>>(results);
			IsLoading = false;
		}
	}
}
