// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityAttribute.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the QualityAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System;

	[Flags]
	public enum QualityAttribute
	{
		CodeQuality = 1, 
		Maintainability = 2, 
		Testability = 4, 
		Modifiability = 8, 
		Reusability = 16, 
		Conformance = 32, 
		Security = 64
	}
}
