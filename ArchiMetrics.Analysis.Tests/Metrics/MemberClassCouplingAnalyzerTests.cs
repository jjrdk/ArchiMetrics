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

namespace ArchiMetrics.Analysis.Tests.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Analysis.Metrics;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
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
			public void WhenCalculatingConstructorCouplingsThenExistingInternalPropertyIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
			}

			[Test]
			public void WhenCalculatingConstructorCouplingsThenExistingInternalMethodIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
			}

			[Test]
			public void WhenCalculatingConstructorCouplingsThenExistingInternalEventIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingConstructorCouplingsThenNonExistingInternalPropertyIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
			}

			[Test]
			public void WhenCalculatingConstructorCouplingsThenNonExistingInternalMethodIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
			}

			[Test]
			public void WhenCalculatingConstructorCouplingsThenNonExistingInternalEventIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetConstructor()).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenExistingInternalPropertyIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenExistingInternalMethodIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenExistingInternalEventIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenNonExistingInternalPropertyIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenNonExistingInternalMethodIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
			}

			[Test]
			public void WhenCalculatingPropertyGetterCouplingsThenNonExistingInternalEventIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.GetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenExistingInternalPropertyIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenExistingInternalMethodIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenExistingInternalEventIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenNonExistingInternalPropertyIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenNonExistingInternalMethodIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
			}

			[Test]
			public void WhenCalculatingPropertySetterCouplingsThenNonExistingInternalEventIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetProperty("PropertyA", SyntaxKind.SetAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenExistingInternalPropertyIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenExistingInternalMethodIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenExistingInternalEventIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenNonExistingInternalPropertyIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenNonExistingInternalMethodIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
			}

			[Test]
			public void WhenCalculatingMethodCouplingsThenNonExistingInternalEventIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetMethod("MethodA")).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenExistingInternalPropertyIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenExistingInternalMethodIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenExistingInternalEventIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenNonExistingInternalPropertyIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenNonExistingInternalMethodIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
			}

			[Test]
			public void WhenCalculatingEventAddCouplingsThenNonExistingInternalEventIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.AddAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.No.Member("MyNamespace.MyClass.EventB"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenExistingInternalPropertyIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.Member("MyNamespace.MyClass.PropertyA"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenExistingInternalMethodIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.Member("MyNamespace.MyClass.MethodA()"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenExistingInternalEventIsRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedEvents, Has.Member("MyNamespace.MyClass.EventA"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenNonExistingInternalPropertyIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedProperties, Has.No.Member("MyNamespace.MyClass.PropertyB"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenNonExistingInternalMethodIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

				Assert.That(localCouplings.UsedMethods, Has.No.Member("MyNamespace.MyClass.MethodB()"));
			}

			[Test]
			public void WhenCalculatingEventRemoveCouplingsThenNonExistingInternalEventIsNotRecognized()
			{
				var testee = CreateTestee();

				var couplings = testee.Calculate(this.GetEvent("EventA2", SyntaxKind.RemoveAccessorDeclaration)).ToList();

				var localCouplings = couplings.Single(c => c.TypeName == "MyClass");

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
				return this.GetNodes<ConstructorDeclarationSyntax>()
					.Single(n => n.ParameterList.Parameters.Count == paramCount);
			}

			private SyntaxNode GetProperty(string name, SyntaxKind kind)
			{
				var property = this.GetNodes<PropertyDeclarationSyntax>()
					.Single(n => n.Identifier.ValueText == name);
				return property.AccessorList.Accessors.Single(x => x.IsKind(kind));
			}

			private SyntaxNode GetMethod(string name)
			{
				return this.GetNodes<MethodDeclarationSyntax>()
					.Single(n => n.Identifier.ValueText == name);
			}

			private SyntaxNode GetEvent(string name)
			{
				return this.GetNodes<EventFieldDeclarationSyntax>()
					.Single(e => e.Declaration.Variables.Any(v => v.Identifier.ValueText == name));
			}

			private SyntaxNode GetEvent(string name, SyntaxKind kind)
			{
				var eventDeclaration = this.GetNodes<EventDeclarationSyntax>()
					.Single(n => n.Identifier.ValueText == name);
				return eventDeclaration.AccessorList.Accessors.Single(x => x.IsKind(kind));
			}

			private IEnumerable<TNode> GetNodes<TNode>()
				where TNode : SyntaxNode
			{
				var document = _solution.Projects.SelectMany(p => p.Documents).First();
				return document
					.GetSyntaxRootAsync()
					.Result
					.DescendantNodes()
					.OfType<TNode>();
			}

			private MemberClassCouplingAnalyzer CreateTestee()
			{
				return new MemberClassCouplingAnalyzer(_solution.Projects.SelectMany(p => p.Documents).First().GetSemanticModelAsync().Result);
			}
		}
	}
}
