// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAvailability.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAvailability type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common
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