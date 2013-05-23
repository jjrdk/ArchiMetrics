// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationRepository.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven.Repositories
{
	using ArchiMeter.Common.Documents;

	using Common;

	using global::Raven.Client;

	public class EvaluationRepository : GenericRepository<EvaluationResultDocument>
	{
		public EvaluationRepository(IFactory<IAsyncDocumentSession> sessionProvider)
			: base(null, sessionProvider)
		{
		}
	}
}