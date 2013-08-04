// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathFilter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PathFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System;
	using ArchiMetrics.Common;

	public class PathFilter
	{
		private readonly Func<ProjectDefinition, bool> _filter;

		public PathFilter(Func<ProjectDefinition, bool> filter)
		{
			_filter = filter;
		}

		public bool Filter(ProjectDefinition definition)
		{
			return _filter(definition);
		}
	}
}
