// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSession.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IDataSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System.Threading.Tasks;

	public interface IDataSession<T> : IReadOnlyDataSession<T>
	{
		Task Store(object item);

		Task Flush();
	}
}
