// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterDocumentation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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