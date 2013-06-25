// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMetricKind.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeMetricKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Metrics
{
	public enum TypeMetricKind
	{
		Unknown = 0, 
		Class = 1, 
		Delegate = 2, 
		Interface = 3, 
		Struct = 4, 
		ValueType = 5
	}
}