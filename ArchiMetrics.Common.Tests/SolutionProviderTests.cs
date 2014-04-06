// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProviderTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Tests
{
	using System.IO;
	using NUnit.Framework;

	public class SolutionProviderTests
	{
		private SolutionProvider _provider;

		[SetUp]
		public void Setup()
		{
			_provider = new SolutionProvider();
		}

		[Test]
		public void CanLoadSolutionFromPath()
		{
			var solutionPath = Path.GetFullPath(@"..\..\..\ArchiMetrics.sln");

			var solution = _provider.Get(solutionPath);

			Assert.NotNull(solution);
		}
	}
}
