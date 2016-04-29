// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllRules.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using Analysis.Common;
	using Analysis.Common.CodeReview;

    public static class AllRules
	{
		public static IEnumerable<ISyntaxEvaluation> GetSyntaxRules(ISpellChecker spellChecker)
		{
			var types = (from type in typeof(AllRules).Assembly.GetTypes()
						 where typeof(ISyntaxEvaluation).IsAssignableFrom(type)
						 where !type.IsInterface && !type.IsAbstract
						 select type).AsArray();
			var simple =
				types.Where(x => x.GetConstructors().Any(c => c.GetParameters().Length == 0))
					.Select(Activator.CreateInstance)
					.Cast<ISyntaxEvaluation>();
			var spelling =
				types.Where(
					x =>
					x.GetConstructors()
						.Any(
							c => c.GetParameters().Length == 1 && typeof(ISpellChecker).IsAssignableFrom(c.GetParameters()[0].ParameterType)))
					.Select(x => Activator.CreateInstance(x, spellChecker))
					.Cast<ISyntaxEvaluation>();

			return simple.Concat(spelling).OrderBy(x => x.ID).AsArray();
		}

		public static IEnumerable<ISymbolEvaluation> GetSymbolRules()
		{
			var types = from type in typeof(AllRules).Assembly.GetTypes()
				   where typeof(ISymbolEvaluation).IsAssignableFrom(type)
				   where !type.IsInterface && !type.IsAbstract
				   select type;

			var simple =
				types.Where(x => x.GetConstructors().Any(c => c.GetParameters().Length == 0))
					.Select(Activator.CreateInstance)
					.Cast<ISymbolEvaluation>();

			return simple.AsArray();
		}
	}
}
