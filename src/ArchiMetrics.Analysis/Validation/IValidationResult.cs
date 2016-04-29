// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationResult.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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