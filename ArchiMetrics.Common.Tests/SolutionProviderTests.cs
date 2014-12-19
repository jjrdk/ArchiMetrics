// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProviderTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using System.Diagnostics.CodeAnalysis;
	using NUnit.Framework;

	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed in teardown.")]
	public class SolutionProviderTests
	{
		private SolutionProvider _provider;

		[SetUp]
		public void Setup()
		{
			_provider = new SolutionProvider();
		}

		[TearDown]
		public void Teardown()
		{
			_provider.Dispose();
		}

		[Test]
		public void CanLoadSolutionFromPath()
		{
			var solutionPath = @"..\..\..\ArchiMetrics.sln".GetLowerCaseFullPath();

			var solution = _provider.Get(solutionPath);

			Assert.NotNull(solution);
		}
	}
}
