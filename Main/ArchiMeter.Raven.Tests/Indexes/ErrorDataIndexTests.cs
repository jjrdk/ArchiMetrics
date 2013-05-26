// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorDataIndexTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorDataIndexTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Raven.Tests.Indexes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using Common.Documents;
	using NUnit.Framework;

	public class ErrorDataIndexTests
	{
		private Func<IEnumerable<EvaluationResultDocument>, IEnumerable<ErrorData>> _map;

		[SetUp]
		public void Setup()
		{
			_map = docs => from doc in docs
						   let groups = doc.Results.GroupBy(x => x.Comment)
						   from grouping in groups
						   let array = grouping.ToArray()
						   let snippets = array
							   .Select(z => new Tuple<string, int>(z.Snippet, z.ErrorCount))
							   .GroupBy(g => g.Item1, g => g.Item2)
							   .Select(g => new Tuple<string, int>(g.Key, g.Sum()))
							   .ToArray()
						   let distinctLoc = snippets.Sum(s => 1)
						   let occurrences = array.Sum(e => e.ErrorCount)
						   let effort = snippets.Sum(z => 1)
						   select new ErrorData
								  {
									  DistinctLoc = distinctLoc, 
									  Effort = effort, 
									  Error = grouping.Key, 
									  Occurrences = occurrences, 
									  ProjectName = doc.ProjectName
								  };
		}

		[Test]
		public void PerformsGroupingOnSingleProject()
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
											 }, 
										 }
						   }
					   };

			var results = _map(docs)
				.ToArray();

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
											 }, 
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

			var results = _map(docs)
				.ToArray();

			Assert.AreEqual(2, results.Length);
		}
	}
}