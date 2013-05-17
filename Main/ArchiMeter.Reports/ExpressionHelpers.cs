namespace ArchiMeter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using ArchiMeter.Common;
	using ArchiMeter.Common.Documents;

	internal static class ExpressionHelpers
	{
		public static Expression<Func<T, bool>> CreateQuery<T>(this IEnumerable<ProjectSettings> projectSettings)
			where T : DataSegment
		{
			var parameter = Expression.Parameter(typeof(T));
			var filter = projectSettings.Aggregate<ProjectSettings, Expression>(
				null,
				(ex, s) =>
					{
						var comparison = Expression.AndAlso(
							Expression.Equal(
								Expression.Property(parameter, "ProjectName"),
								Expression.Property(Expression.Constant(s), "Name")),
							Expression.Equal(
								Expression.Property(parameter, "Date"),
								Expression.Property(Expression.Constant(s), "Date")));
						return ex == null
							       ? comparison
							       : Expression.OrElse(ex, comparison);
					});

			var lambda = Expression.Lambda<Func<T, bool>>(filter, parameter);
			return lambda;
		}
	}
}