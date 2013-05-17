// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestErrorGraphViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestErrorGraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMeter.Common;

	public class TestErrorGraphViewModel : WorkspaceViewModel
	{
		public TestErrorGraphViewModel(ICodeErrorRepository repository)
		{
			repository.GetErrorsAsync()
					  .ContinueWith(DisplayErrors);
		}

		public IList<KeyValuePair<int, int>> Errors { get; private set; }

		private void DisplayErrors(Task<IEnumerable<EvaluationResult>> task)
		{
			IsLoading = true;
			var results = task.Result.Where(x => x.Comment == "Multiple asserts found in test.")
							  .GroupBy(x => x.ErrorCount).Select(x => new KeyValuePair<int, int>(x.Key, x.Count())).OrderBy(x => x.Key);
			Errors = new List<KeyValuePair<int, int>>(results);
			IsLoading = false;
		}
	}
}