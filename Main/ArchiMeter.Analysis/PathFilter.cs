namespace ArchiMeter.Analysis
{
	using System;

	using ArchiMeter.Common;

	public class PathFilter
	{
		private readonly Func<ProjectDefinition, bool> _filter;

		public PathFilter(Func<ProjectDefinition, bool> filter)
		{
			this._filter = filter;
		}

		public bool Filter(ProjectDefinition definition)
		{
			return this._filter(definition);
		}
	}
}