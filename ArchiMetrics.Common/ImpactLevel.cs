// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImpactLevel.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ImpactLevel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	public enum ImpactLevel
	{
		Project = 0, 
		Namespace = 1, 
		Type = 2, 
		Member = 3, 
		Line = 4, 
		Node = 5
	}
}
