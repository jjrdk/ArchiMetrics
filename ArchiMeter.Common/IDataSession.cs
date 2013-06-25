// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSession.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IDataSession type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common
{
	using System.Threading.Tasks;

	public interface IDataSession<T> : IReadOnlyDataSession<T>
	{
		Task Store(object item);

		Task Flush();
	}
}