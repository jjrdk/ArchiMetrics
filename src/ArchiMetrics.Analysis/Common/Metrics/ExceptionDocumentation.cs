// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionDocumentation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExceptionDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
	public class ExceptionDocumentation
	{
		public ExceptionDocumentation(string exceptionType, string description)
		{
			ExceptionType = exceptionType;
			Description = description;
		}

		public string ExceptionType { get; }

		public string Description { get; }
	}
}