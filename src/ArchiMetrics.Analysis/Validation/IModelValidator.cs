// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelValidator.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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
	using Common.Structure;

    public interface IModelValidator
	{
		Task<IEnumerable<IValidationResult>> Validate(string solutionPath, IEnumerable<IModelRule> modelVertices, IEnumerable<TransformRule> transformRules, CancellationToken cancellationToken);
	}
}