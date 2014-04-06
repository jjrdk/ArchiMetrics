namespace ArchiMetrics.CodeReview.Rules
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;

	public	static class AllRules
	{
		public static IEnumerable<Type> GetRules()
		{
			return from type in typeof(AllRules).Assembly.GetTypes()
				   where typeof(IEvaluation).IsAssignableFrom(type)
				   where !type.IsInterface && !type.IsAbstract
				   select type;
		} 
	}
}
