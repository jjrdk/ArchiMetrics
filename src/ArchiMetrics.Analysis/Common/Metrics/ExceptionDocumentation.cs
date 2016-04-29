// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionDocumentation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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

		public string ExceptionType { get; private set; }

		public string Description { get; private set; }
	}
}