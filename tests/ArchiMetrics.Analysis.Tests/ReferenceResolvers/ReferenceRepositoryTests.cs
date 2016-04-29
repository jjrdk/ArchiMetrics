// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceRepositoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReferenceRepositoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Tests.ReferenceResolvers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using ArchiMetrics.Analysis.ReferenceResolvers;
    using Common;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public sealed class ReferenceRepositoryTests
    {
        private ReferenceRepositoryTests()
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed in teardown.")]
        public class GivenAReferenceRepository : SolutionTestsBase, IDisposable
        {
            private readonly ReferenceRepository _sut;
            private readonly Solution _solution;

            public GivenAReferenceRepository()
            {
                const string Code = @"namespace Test
{
	using System;

	public class TestClass
	{
		private object _number = new object();

		public object GetNumber()
		{
			return _number;
		}
	}
}";
                _solution = CreateSolution(Code);
                _sut = new ReferenceRepository(_solution);
            }

            public void Dispose()
            {
                _sut.Dispose();
            }

            [Fact]
            public async Task WhenResolvingReferencesThenResolvesAllReferences()
            {
                var project = _solution.Projects.First();
                var compilation = await project.GetCompilationAsync();
                var document = project.Documents.First();
                var root = await document.GetSyntaxRootAsync();
                var model = compilation.GetSemanticModel(root.SyntaxTree);
                var symbol = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Select(x => model.GetDeclaredSymbol(x) as IMethodSymbol)
                    .Select(x => x.ReturnType)
                    .First();

                var location = _sut.Get(symbol).AsArray();

                Assert.Equal(3, location.Length);
            }
        }
    }
}
