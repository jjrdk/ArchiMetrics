// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefinedRules.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DefinedRules type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

	public class DefinedRules : IEnumerable<ICodeEvaluation>
	{
		private static readonly IEnumerable<ICodeEvaluation> DefinedTypes = typeof(DefinedRules)
			.Assembly
			.GetTypes()
			.Where(t => typeof(ICodeEvaluation).IsAssignableFrom(t))
			.Where(t => !t.IsInterface && !t.IsAbstract)
			.Select(Activator.CreateInstance)
			.Cast<ICodeEvaluation>()
			.ToArray();

		// private static readonly IEnumerable<ICodeEvaluation> DefinedTypes = new[] { new TypeObfuscationRule() };
		private DefinedRules()
		{
		}

		public static IEnumerable<ICodeEvaluation> Default
		{
			get { return DefinedTypes; }
		}

		public IEnumerator<ICodeEvaluation> GetEnumerator()
		{
			return DefinedTypes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
