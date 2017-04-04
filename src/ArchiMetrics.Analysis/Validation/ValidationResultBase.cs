// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationResultBase.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ValidationResultBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Validation
{
    using Common.Structure;

    public abstract class ValidationResultBase : IValidationResult
	{
		protected ValidationResultBase(bool passed, IModelNode vertex)
		{
			Passed = passed;
			Vertex = vertex;
		}

		public bool Passed { get; }

		public abstract string Value { get; }

		public IModelNode Vertex { get; set; }
	}
}