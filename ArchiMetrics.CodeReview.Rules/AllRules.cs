// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllRules.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the AllRules type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
