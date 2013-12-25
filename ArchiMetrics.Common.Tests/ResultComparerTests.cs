// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultComparerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ResultComparerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Tests
{
	using System;
	using ArchiMetrics.Common.CodeReview;
	using NUnit.Framework;

	public class ResultComparerTests
	{
		private ResultComparerTests()
		{
		}

		public class GivenAResultComparer
		{
			private ResultComparer _comparer;
			private EvaluationResult _result;

			[SetUp]
			public void Setup()
			{
				_comparer = new ResultComparer();
				_result = new EvaluationResult
						  {
							  ErrorCount = 1,
							  FilePath = "filepath",
							  ImpactLevel = ImpactLevel.Line,
							  LinesOfCodeAffected = 1,
							  Namespace = "namespace",
							  ProjectPath = "projectpath",
							  Quality = CodeQuality.Good,
							  QualityAttribute = QualityAttribute.CodeQuality,
							  Snippet = "snippet",
							  Suggestion = "suggestion",
							  Title = "title",
							  TypeKind = "kind",
							  TypeName = "typename"
						  };
			}

			[Test]
			public void WhenGettingHashCodeForNullObjectThenThrows()
			{
				Assert.Throws<ArgumentNullException>(() => _comparer.GetHashCode(null));
			}

			[Test]
			public void WhenGettingHashCodeForNonNullObjectThenReturnsHashCode()
			{
				Assert.DoesNotThrow(() => _comparer.GetHashCode(_result));
			}

			[Test]
			public void WhenComparingTwoNullResultThenAreEqual()
			{
				Assert.IsTrue(_comparer.Equals(null, null));
			}

			[Test]
			public void WhenComparingResultsWithSameValuesThenAreEqual()
			{
				var other = new EvaluationResult
				{
					ErrorCount = 1,
					FilePath = "filepath",
					ImpactLevel = ImpactLevel.Line,
					LinesOfCodeAffected = 1,
					Namespace = "namespace",
					ProjectPath = "projectpath",
					Quality = CodeQuality.Good,
					QualityAttribute = QualityAttribute.CodeQuality,
					Snippet = "snippet",
					Suggestion = "suggestion",
					Title = "title",
					TypeKind = "kind",
					TypeName = "typename"
				};

				Assert.IsTrue(_comparer.Equals(_result, other));
			}

			[TestCase("ErrorCount", 2)]
			[TestCase("FilePath", "other")]
			[TestCase("ImpactLevel", ImpactLevel.Member)]
			[TestCase("LinesOfCodeAffected", 2)]
			[TestCase("Namespace", "other")]
			[TestCase("ProjectPath", "other")]
			[TestCase("Quality", CodeQuality.Broken)]
			[TestCase("QualityAttribute", QualityAttribute.Conformance)]
			[TestCase("Snippet", "other")]
			[TestCase("Suggestion", "other")]
			[TestCase("Title", "other")]
			[TestCase("TypeKind", "other")]
			[TestCase("TypeName", "other")]
			public void WhenComparingResultsWithDifferentValuesThenAreDifferent(string propertyName, object value)
			{
				var other = new EvaluationResult
				{
					ErrorCount = 1,
					FilePath = "filepath",
					ImpactLevel = ImpactLevel.Line,
					LinesOfCodeAffected = 1,
					Namespace = "namespace",
					ProjectPath = "projectpath",
					Quality = CodeQuality.Good,
					QualityAttribute = QualityAttribute.CodeQuality,
					Snippet = "snippet",
					Suggestion = "suggestion",
					Title = "title",
					TypeKind = "kind",
					TypeName = "typename"
				};

				typeof(EvaluationResult).GetProperty(propertyName).SetValue(other, value);

				Assert.IsFalse(_comparer.Equals(_result, other));
			}
		}
	}
}