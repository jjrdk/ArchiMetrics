// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectComparerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectComparerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Tests
{
	using Moq;
	using NUnit.Framework;
	using Roslyn.Services;

	public class ProjectComparerTests
	{
		private ProjectComparerTests()
		{
		}

		public class GivenAProjectComparer
		{
			[Test]
			public void WhenComparingProjectsWithDifferentFilePathThenAreDifferent()
			{
				var first = new Mock<IProject>();
				first.SetupGet(x => x.FilePath).Returns("a");
				var second = new Mock<IProject>();
				second.SetupGet(x => x.FilePath).Returns("b");

				var areEqual = ProjectComparer.Default.Equals(first.Object, second.Object);

				Assert.IsFalse(areEqual);
			}
		}
	}
}