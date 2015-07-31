// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeParameterDocumentation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeParameterDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	public class TypeParameterDocumentation
	{
		public TypeParameterDocumentation(string typeParameterName, string constraint, string description)
		{
			TypeParameterName = typeParameterName;
			Constraint = constraint;
			Description = description;
		}

		public string TypeParameterName { get; private set; }

		public string Constraint { get; private set; }

		public string Description { get; private set; }
	}
}