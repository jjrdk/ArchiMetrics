// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrorRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeErrorRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;

	public class CodeErrorRepository : ICodeErrorRepository, IResetable
	{
		private readonly IAvailableRules _availableRules;
		private readonly ConcurrentDictionary<string, Task<EvaluationResult[]>> _evaluations;
		private readonly INodeInspector _inspector;
		private readonly IProvider<string, Solution> _solutionProvider;

		public CodeErrorRepository(
			IProvider<string, Solution> solutionProvider, 
			INodeInspector inspector, 
			IAvailableRules availableRules)
		{
			_evaluations = new ConcurrentDictionary<string, Task<EvaluationResult[]>>();
			_solutionProvider = solutionProvider;
			_inspector = inspector;
			_availableRules = availableRules;
		}

		~CodeErrorRepository()
		{
			Dispose(false);
		}

		public async Task<IEnumerable<EvaluationResult>> GetErrors(string solutionFile, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrWhiteSpace(solutionFile))
			{
				return Enumerable.Empty<EvaluationResult>();
			}

			var results = await _evaluations.GetOrAdd(solutionFile, LoadEvaluationResults);

			var availableRules = new HashSet<string>(_availableRules.Select(x => x.Title));
			return cancellationToken.IsCancellationRequested
					   ? Enumerable.Empty<EvaluationResult>()
					   : results.Where(x => availableRules.Contains(x.Title)).ToArray();
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public void Reset()
		{
			_evaluations.Clear();
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_evaluations.Clear();
			}
		}

		private async Task<EvaluationResult[]> LoadEvaluationResults(string path)
		{
			var solution = _solutionProvider.Get(path);
			return (await _inspector.Inspect(solution)).ToArray();
		}
	}
}
