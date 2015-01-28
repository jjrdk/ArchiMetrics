// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ExtensionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Tests
{
	using System;
	using Moq;
	using NUnit.Framework;

	public class ExtensionsTests
	{
		[Test]
		public void WhenDisposingDisposableObjectThenCallsDispose()
		{
			var disposableMock = new Mock<IDisposable>();
			disposableMock.Object.DisposeNotNull();

			disposableMock.Verify(x => x.Dispose(), Times.Once);
		}
	}
}