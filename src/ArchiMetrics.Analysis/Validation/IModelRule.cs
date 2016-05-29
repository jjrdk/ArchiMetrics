// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelRule.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IModelRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Common.Structure;

    public interface IModelRule
	{
		Task<IEnumerable<IValidationResult>> Validate(IModelNode modelTree);
	}
}