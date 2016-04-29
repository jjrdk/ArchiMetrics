// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterDocumentation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
	public class ParameterDocumentation
	{
		public ParameterDocumentation(string parameterName, string parameterType, string description)
		{
			ParameterName = parameterName;
			ParameterType = parameterType;
			Description = description;
		}

		public string ParameterName { get; private set; }

		public string ParameterType { get; private set; }

		public string Description { get; private set; }
	}
}