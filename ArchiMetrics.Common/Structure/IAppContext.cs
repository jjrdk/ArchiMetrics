// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppContext.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IAppContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ArchiMetrics.Common.Structure
{
	using System.ComponentModel;

	public interface IAppContext : INotifyPropertyChanged, IDisposable
	{
		string Path { get; set; }

		EdgeSource Source { get; set; }

		bool IncludeCodeReview { get; set; }

		string RulesSource { get; set; }
	}
}
