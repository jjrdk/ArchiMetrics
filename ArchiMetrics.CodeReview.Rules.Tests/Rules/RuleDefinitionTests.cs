// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleDefinitionTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RuleDefinitionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.CodeReview;
	using NUnit.Framework;

	public class RuleDefinitionTests
	{
		[TestCaseSource(typeof(RuleProvider))]
		public void RuleHasTitle(Type type)
		{
			var rule = (IEvaluation)Activator.CreateInstance(type);

			Assert.False(string.IsNullOrWhiteSpace(rule.Title));
		}

		[TestCaseSource(typeof(RuleProvider))]
		public void RuleHasSuggestion(Type type)
		{
			var rule = (IEvaluation)Activator.CreateInstance(type);

			Assert.False(string.IsNullOrWhiteSpace(rule.Suggestion));
		}

		private class RuleProvider : IEnumerable<Type>
		{
			/// <summary>
			/// Returns an enumerator that iterates through the collection.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
			/// </returns>
			public IEnumerator<Type> GetEnumerator()
			{
				return typeof(ReportUtils).Assembly
					.GetTypes()
					.Where(t => typeof(IEvaluation).IsAssignableFrom(t))
					.Where(t => !t.IsAbstract)
					.Where(t => !t.IsInterface)
					.Where(t => t.GetConstructors().Any(c => !c.GetParameters().Any()))
					.GetEnumerator();
			}

			/// <summary>
			/// Returns an enumerator that iterates through a collection.
			/// </summary>
			/// <returns>
			/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
			/// </returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}
