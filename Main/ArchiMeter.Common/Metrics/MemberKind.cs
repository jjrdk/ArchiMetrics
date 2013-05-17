// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberKind.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Metrics
{
	public enum MemberKind
	{
		Method = 0, 
		Constructor = 1, 
		Destructor = 2, 
		GetProperty = 3, 
		SetProperty = 4, 
		AddEventHandler = 5, 
		RemoveEventHandler = 6
	}
}