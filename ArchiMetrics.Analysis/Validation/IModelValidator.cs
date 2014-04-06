// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelValidator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IModelValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.Structure;

	public interface IModelValidator
	{
		Task<IEnumerable<IValidationResult>> Validate(string solutionPath, IEnumerable<IModelRule> modelVertices, IEnumerable<TransformRule> transformRules, CancellationToken cancellationToken);
	}
}