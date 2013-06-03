// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrorGraphViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeErrorGraphViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.ViewModel
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using ArchiMeter.Common;

	public class CodeErrorGraphViewModel : ViewModelBase
	{
		public CodeErrorGraphViewModel(ICodeErrorRepository repository)
		{
			repository.GetErrorsAsync().ContinueWith(this.DisplayErrors);
		}

		public IList<KeyValuePair<string, int>> Errors { get; private set; }

		private void DisplayErrors(Task<IEnumerable<EvaluationResult>> task)
		{
			this.IsLoading = true;
			var results = task.Result.GroupBy(x => x.Comment).Select(x => new KeyValuePair<string, int>(x.Key, x.Count())).OrderBy(x => x.Key);
			this.Errors = new List<KeyValuePair<string, int>>(results);
			this.IsLoading = false;
		}
	}
}