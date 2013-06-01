// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementToTestAnalyzerTests.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RequirementToTestAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Analysis.Tests
{
	using System.Linq;
	using System.Threading;
	using System.Xml.Linq;
	using Common;
	using Moq;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class RequirementToTestAnalyzerTests
	{
		private IRequirementTestAnalyzer _analyzer;
		private SyntaxNode _rootNode;

		[SetUp]
		public void Setup()
		{
			var tree = SyntaxTree.ParseText(@"public class TestClass {
	[TestProperty(TC.Requirement, ""1"")]
	[TestMethod]
	public void SomeTest() {
		// Blah
	}
}");
			_rootNode = tree.GetRoot();

			var mockDocument = new Mock<IDocument>();
			mockDocument.SetupGet(x => x.FilePath).Returns("a");
			mockDocument.Setup(x => x.GetSyntaxRoot(It.IsAny<CancellationToken>())).Returns(() => _rootNode);
			var mockProject = new Mock<IProject>();
			mockProject.SetupGet(x => x.FilePath).Returns("a");
			mockProject.SetupGet(x => x.Documents).Returns(() => new[] { mockDocument.Object });
			var mockSolution = new Mock<ISolution>();
			mockSolution.SetupGet(x => x.Projects).Returns(new[] { mockProject.Object });
			var mockProvider = new Mock<IProvider<string, IProject>>();
			mockProvider.Setup(x => x.GetAll(It.IsAny<string>())).Returns(() => new[] { mockProject.Object });
			_analyzer = new RequirementTestAnalyzer(mockProvider.Object);
		}

		[Test]
		public void CanResolveRequirementIdsInAttribute()
		{
			var result = _analyzer.GetTestData(@"c:\");

			Assert.IsNotEmpty(result);
		}

		[Test]
		public void CanResolveTestForRequirementIds()
		{
			var result = _analyzer.GetRequirementTests(@"c:\");

			Assert.IsNotEmpty(result);
		}

		[Test]
		public void WhenNoRequirementIsSpecifiedThenTestIsIncludedInReport()
		{
			var result = _analyzer.GetRequirementTests(@"c:\");

			Assert.IsNotEmpty(result);
		}

		[Test]
		public void CanWriteReport()
		{
			const string Expected = @"<Root>
  <Requirement Count=""1"">
    <Tests>
      <Test><![CDATA[	[TestProperty(TC.Requirement, ""1"")]
	[TestMethod]
	public void SomeTest() {
		// Blah
	}
]]></Test>
    </Tests>
  </Requirement>
</Root>";
			var reports = _analyzer.GetRequirementTests(@"c:\").ToArray();
			var elements = reports.Select(
				r => new XElement(
						 "Requirement", 
						 new XAttribute("Count", r.CoveringTests.Count()), 
						 new XElement("Tests", r.CoveringTests.Select(t => new XElement("Test", new XCData(t))))));
			var doc = new XDocument(new XElement("Root", elements));

			var report = doc.ToString(SaveOptions.OmitDuplicateNamespaces);
			Assert.AreEqual(Expected, report);
		}
	}
}