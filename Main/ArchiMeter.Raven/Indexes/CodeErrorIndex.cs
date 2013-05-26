// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrorIndex.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeErrorIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Client.Indexes;

	public class CodeErrorIndex : AbstractIndexCreationTask<EvaluationResultDocument, CodeErrors>
	{
		public CodeErrorIndex()
		{
			Map = evals => from eval in evals
						   from result in eval.Results
						   select new
								  {
									  eval.ProjectName,
									  eval.ProjectVersion,
									  Error = result.Comment,
									  result.Namespace,
									  Snippets = new[]
												 {
													 new
													 {
														 result.Snippet, result.FilePath
													 }
												 }
								  };

			Reduce = errors => from error in errors
							   group error by error.ProjectName + error.ProjectVersion + error.Error
								   into errorGroup
								   let first = errorGroup.First()
								   select new
										  {
											  first.ProjectName,
											  first.ProjectVersion,
											  first.Error,
											  first.Namespace,
											  Snippets = errorGroup.SelectMany(g => g.Snippets).ToArray()
										  };
		}
	}
}