// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocCounterTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LocCounterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Analysis.Tests
{
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public class LocCounterTests
	{
		private SLoCCounter _counter;

		[SetUp]
		public void Setup()
		{
			_counter = new SLoCCounter();
		}

		[Test]
		public void CanCountSyntax()
		{
			var syntax = SyntaxTree.ParseText(@"public class ParseClass
{ 
	// A comment
	int x = 0;
}");

			Assert.AreEqual(2, _counter.Count(syntax.GetRoot()));
		}
	}
}
