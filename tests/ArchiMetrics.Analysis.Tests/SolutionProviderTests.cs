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

namespace ArchiMetrics.Analysis.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Common;
    using Xunit;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed in teardown.")]
    public class SolutionProviderTests : IDisposable
    {
        private readonly SolutionProvider _provider;

        public SolutionProviderTests()
        {
            _provider = new SolutionProvider();
        }

        public void Dispose()
        {
            _provider.Dispose();
        }

        [Fact]
        public void CanLoadSolutionFromPath()
        {
            var solutionPath = @"..\..\..\ArchiMetrics.sln".GetLowerCaseFullPath();

            var solution = _provider.Get(solutionPath);

            Assert.NotNull(solution);
        }
    }
}
