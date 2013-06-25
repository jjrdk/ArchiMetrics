// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetricKind.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetricKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Metrics
{
	public enum MemberMetricKind
	{
		Unknown = 0, 
		Method = 1, 
		PropertyAccessor = 2, 
		EventAccessor = 3
	}
}