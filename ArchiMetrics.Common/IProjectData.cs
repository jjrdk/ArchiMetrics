// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectData.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IProjectData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	public interface IProjectData
	{
		string Name { get; }

		string Path { get; }
	}
}
