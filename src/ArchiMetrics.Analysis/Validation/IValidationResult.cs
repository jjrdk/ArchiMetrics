// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationResult.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IValidationResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
    using Common.Structure;

    public interface IValidationResult
	{
		bool Passed { get; }

		string Value { get; }

		IModelNode Vertex { get; }
	}
}