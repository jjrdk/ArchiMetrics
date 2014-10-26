// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberClassCouplingAnalyzerTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberClassCouplingAnalyzerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ArchiMetrics.Common.Metrics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using Microsoft.CodeAnalysis;
	using NUnit.Framework;

	public sealed class MemberClassCouplingAnalyzerTests
	{
		private MemberClassCouplingAnalyzerTests()
		{
		}

		public class GivenAMemberClassCouplingAnalyzer
		{
			private Solution _solution;

			[SetUp]
			public void Setup()
			{
				_solution = CreateSolution(@"
namespace MyNamespace
{
	using System;
	public class MyClass
	{
		public MyClass()
		{
			var tmp = PropertyA;
			MethodA();
			EventA(this, EventArgs.Empty);
		}

		public int PropertyA
		{
			get
			{
				var tmp = PropertyA;
				MethodA();
				EventA(this, EventArgs.Empty);

				return 1;
			}
			set
			{
				var tmp = PropertyA;
				MethodA();
				EventA(this, EventArgs.Empty);
			}
		}

		public void MethodA()
		{
			var tmp = PropertyA;
			MethodA();
			EventA(this, EventArgs.Empty);
		}

		public event EventHandler EventA;
		public event EventHandler EventA2
		{
			add
			{
				var tmp = PropertyA;
				MethodA();
				EventA(this, EventArgs.Empty);
			}
			remove
			{
				var tmp = PropertyA;
				MethodA();
				EventA(this, EventArgs.Empty);
			}
		};

		public MyClass(int B) { }
		public int PropertyB { get { return 1; } }
		public void MethodB() {}
		public event EventHandler EventB;
	}
}");
			}

			[Test]
			public void WhenCalculatingConstructorCouplingsThenExistingInternalDependenciesAreRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingConstructorCouplingsThenNonExistingInternalDependenciesAreNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			[Ignore("Not implemented yet.")]
			public void WhenCalculatingConstructorCouplingsThenExistingExternalDependenciesAreRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			[Ignore("Not implemented yet.")]
			public void WhenCalculatingConstructorCouplingsThenNonExistingExternalDependenciesAreNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenExistingInternalDependenciesAreRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenNonExistingInternalDependenciesAreNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenExistingInternalDependenciesAreRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenNonExistingInternalDependenciesAreNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenExistingInternalDependenciesAreRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenNonExistingInternalDependenciesAreNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenExistingInternalDependenciesAreRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenNonExistingInternalDependenciesAreNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenExistingInternalDependenciesAreRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenNonExistingInternalDependenciesAreNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingEventCouplingsThenIncludesEventDependencies()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA")).ToList();

				Assert.That(couplings, Has.Some.Matches<ITypeCoupling>(x => x.Namespace == "System" && x.TypeName == "EventHandler" && x.Assembly == "mscorlib"));
			}

			private Solution CreateSolution(params string[] code)
			{
				using (var workspace = new CustomWorkspace())
				{
					workspace.AddSolution(
						SolutionInfo.Create(
							SolutionId.CreateNewId("Semantic"), 
							VersionStamp.Default));
					var x = 1;
					var projectId = ProjectId.CreateNewId("testcode");
					var solution = workspace.CurrentSolution.AddProject(projectId, "testcode", "testcode.dll", LanguageNames.CSharp)
						.AddMetadataReference(projectId, new MetadataFileReference(typeof(object).Assembly.Location));
					solution = code.Aggregate(
						solution, 
						(sol, c) => sol.AddDocument(DocumentId.CreateNewId(projectId), string.Format("TestClass{0}.cs", x++), c));

					return solution;
				}
			}

			private SyntaxNode GetConstructor(int paramCount = 0)
			{
				var document = _solution.Projects.SelectMany(p => p.Documents).First();
				var constructor = document
					.GetSyntaxRootAsync()
					.Result
					.DescendantNodes()
					.OfType<ConstructorDeclarationSyntax>()
					.Single(n => n.ParameterList.Parameters.Count == paramCount);
				return constructor;
			}

			private SyntaxNode GetProperty(string name, SyntaxKind kind)
			{
				var document = _solution.Projects.SelectMany(p => p.Documents).First();
				var property = document
					.GetSyntaxRootAsync()
					.Result
					.DescendantNodes()
					.OfType<PropertyDeclarationSyntax>()
					.Single(n => n.Identifier.ValueText == name);
				return property.AccessorList.Accessors.Single(x => x.IsKind(kind));
			}

			private SyntaxNode GetMethod(string name)
			{
				var document = _solution.Projects.SelectMany(p => p.Documents).First();
				var allProperties = document
					.GetSyntaxRootAsync()
					.Result
					.DescendantNodes()
					.OfType<MethodDeclarationSyntax>();
				return allProperties.Single(n => n.Identifier.ValueText == name);
			}

			private SyntaxNode GetEvent(string name)
			{
				var document = _solution.Projects.SelectMany(p => p.Documents).First();
				var allEventFields = document
					.GetSyntaxRootAsync()
					.Result
					.DescendantNodes()
					.OfType<EventFieldDeclarationSyntax>()
					.ToList();
				var eventDeclaration = allEventFields
					.Single(e => (e.Declaration.Variables.Any(v => v.Identifier.ValueText == name)));
				return eventDeclaration;
			}

			private SyntaxNode GetEvent(string name, SyntaxKind kind)
			{
				var document = _solution.Projects.SelectMany(p => p.Documents).First();
				var allMembers = document
					.GetSyntaxRootAsync()
					.Result
					.DescendantNodes().ToList();
				var eventDeclarations = allMembers.OfType<EventDeclarationSyntax>();
				var eventDeclaration = eventDeclarations.Single(n => n.Identifier.ValueText == name);
				return eventDeclaration.AccessorList.Accessors.Single(x => x.IsKind(kind));
			}

			private MemberClassCouplingAnalyzer CreateTestee()
			{
				return new MemberClassCouplingAnalyzer(_solution.Projects.SelectMany(p => p.Documents).First().GetSemanticModelAsync().Result);
			}
		}
	}
}
