// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationResultDocumentIndex.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationResultDocumentIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;
	using Common.Documents;
	using global::Raven.Abstractions.Indexing;
	using global::Raven.Client.Indexes;

	public class EvaluationResultDocumentIndex : AbstractIndexCreationTask<EvaluationResultDocument>
	{
		public EvaluationResultDocumentIndex()
		{
			Map = ed => from e in ed
						select new
								   {
									   ProjectName = e.ProjectName,
									   ProjectVersion = e.ProjectVersion
								   };

			Store(e => e.ProjectName, FieldStorage.Yes);
			Store(e => e.ProjectVersion, FieldStorage.Yes);
		}
	}
}