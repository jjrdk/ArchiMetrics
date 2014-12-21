// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResetable.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IResetable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	/// <summary>
	/// Defines the interface for items which can be reset to their original state.
	/// </summary>
	public interface IResetable
	{
		/// <summary>
		/// Resets the instance to the original state.
		/// </summary>
		void Reset();
	}
}
