// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataComparerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectDataComparerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.CodeReview.Tests
{
	using NUnit.Framework;

	public sealed class ProjectDataComparerTests
	{
		private ProjectDataComparerTests()
		{
		}

		public class GivenAProjectDataComparer
		{
			[Test]
			public void WhenComparingHashCodesThenEqualProjectsHaveEqualHashCodes()
			{

				var project1 = new ProjectData
				{
					ProjectName = "Project 1",
					ProjectVersion = "1"
				};

				var project2 = new ProjectData
				{
					ProjectName = "Project 1",
					ProjectVersion = "1"
				};
				var hashCode1 = ProjectDataComparer.Instance.GetHashCode(project1);
				var hashCode2 = ProjectDataComparer.Instance.GetHashCode(project2);

				Assert.AreEqual(hashCode1, hashCode2);
			}

			[Test]
			public void WhenComparingTwoProjectsWithSameFilePathThenProjectAreEqual()
			{
				var project1 = new ProjectData
							   {
								   ProjectName = "Project 1",
								   ProjectVersion = "1"
							   };

				var project2 = new ProjectData
							   {
								   ProjectName = "Project 1",
								   ProjectVersion = "1"
							   };

				Assert.IsTrue(ProjectDataComparer.Instance.Equals(project1, project2));
			}
		}
	}
}
