// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAvailability.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	/// <summary>
	/// Defines the interface for instances which are temporarlly available.
	/// </summary>
	public interface IAvailability
	{
		/// <summary>
		/// Gets or sets whether the instance is available.
		/// </summary>
		bool IsAvailable { get; set; }

		/// <summary>
		/// Gets the title of the instance.
		/// </summary>
		string Title { get; }
	}
}