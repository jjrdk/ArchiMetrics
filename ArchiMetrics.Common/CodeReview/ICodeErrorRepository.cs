namespace ArchiMetrics.Common.CodeReview
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface ICodeErrorRepository : IDisposable
	{
		Task<IEnumerable<EvaluationResult>> GetErrors(string solutionPath, CancellationToken cancellationToken);
	}
}
