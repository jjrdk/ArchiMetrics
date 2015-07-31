// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathExtensionsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PathExtensionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Tests
{
	using NUnit.Framework;

	public class PathExtensionsTests
	{
		[Test]
		public void WhenGettingFileNameWithoutExtensionThenReturnsName()
		{
			const string Path = @".\ArchiMetrics.Common.Tests.dll";

			var filename = Path.GetFileNameWithoutExtension();

			Assert.AreEqual("ArchiMetrics.Common.Tests", filename);
		}

		[Test]
		public void WhenGettingLowerCaseExtensionThenReturnsResult()
		{
			const string Path = @".\ArchiMetrics.Common.Tests.DlL";

			var filename = Path.GetLowerCaseExtension();

			Assert.AreEqual(".dll", filename);
		}
	}
}