// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrorIndexTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeErrorIndexTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven.Tests.Indexes
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Common;
	using Common.Documents;
	using NUnit.Framework;
	using Raven.Indexes;

	public class CodeErrorIndexTests : IndexTestBase<EvaluationResultDocument, CodeErrors, CodeErrorIndexTests.FakeCodeErrorIndex>
	{
		[Test]
		public void PerformsGroupingOnSingleProject()
		{
			var docs = new[]
					   {
						   new EvaluationResultDocument
						   {
							   ProjectName = "Test", 
							   ProjectVersion = "1", 
							   Results = new[]
										 {
											 new EvaluationResult
											 {
												 Comment = "Error", 
												 Snippet = "a"
											 }, 
											 new EvaluationResult
											 {
												 Comment = "Error", 
												 Snippet = "b"
											 } 
										 }
						   }
					   };

			var results = PerformMapReduce(docs);

			Assert.AreEqual(1, results.Length);
		}

		[Test]
		public void PerformsGroupingOnTwoProjects()
		{
			var docs = new[]
					   {
						   new EvaluationResultDocument
						   {
							   ProjectName = "Test", 
							   Results = new[]
										 {
											 new EvaluationResult
											 {
												 Comment = "Error", 
												 Snippet = "a"
											 }, 
											 new EvaluationResult
											 {
												 Comment = "Error", 
												 Snippet = "b"
											 } 
										 }
						   }, 
						   new EvaluationResultDocument
						   {
							   ProjectName = "Test2", 
							   Results = new[]
										 {
											 new EvaluationResult
											 {
												 Comment = "Error", 
												 Snippet = "a"
											 }, 
											 new EvaluationResult
											 {
												 Comment = "Error", 
												 Snippet = "b"
											 }, 
										 }
						   }, 
					   };

			var results = PerformMapReduce(docs);

			Assert.AreEqual(2, results.Length);
		}

		public class FakeCodeErrorIndex : CodeErrorIndex, ITestIndex<EvaluationResultDocument, CodeErrors>
		{
			public Expression<Func<IEnumerable<EvaluationResultDocument>, IEnumerable>> GetMap()
			{
				return Map;
			}

			public Expression<Func<IEnumerable<CodeErrors>, IEnumerable>> GetReduce()
			{
				return Reduce;
			}
		}
	}
}
