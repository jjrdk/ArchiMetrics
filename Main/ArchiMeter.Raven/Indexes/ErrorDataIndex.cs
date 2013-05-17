// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorDataIndex.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorDataIndex type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven.Indexes
{
	using System.Linq;

	using ArchiMeter.Common.Documents;

	using global::Raven.Abstractions.Indexing;
	using global::Raven.Client.Indexes;

	public class ErrorDataIndex : AbstractIndexCreationTask<ErrorData>
	{
		public ErrorDataIndex()
		{
			Map = ed => from e in ed
						select new
								   {
									   e.ProjectName, 
									   e.ProjectVersion
								   };

			Store(e => e.ProjectName, FieldStorage.Yes);
			Store(e => e.ProjectVersion, FieldStorage.Yes);
		}
	}
}