// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationResultBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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

		public bool Passed { get; private set; }

		public abstract string Value { get; }

		public IModelNode Vertex { get; set; }
	}
}