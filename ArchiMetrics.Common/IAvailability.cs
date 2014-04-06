// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAvailability.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAvailability type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	public interface IAvailability
	{
		bool IsAvailable { get; set; }

		string Title { get; }
	}
}