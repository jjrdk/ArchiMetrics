// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxInspectorTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IMarkerInterface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Tests.Rules
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Code;
	using Common;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public sealed class SyntaxInspectorTests
	{
		private SyntaxInspectorTests()
		{
		}

		private static Task<IEnumerable<EvaluationResult>> PerformInspection(string code, Type evaluatorType)
		{
			var inspector = new NodeInspector(new[] { (ICodeEvaluation)Activator.CreateInstance(evaluatorType) });
			var tree = SyntaxTree.ParseText("public class ParseClass { " + code + " }");

			var task = inspector.Inspect(string.Empty, tree.GetRoot(), null, null);
			return task;
		}

		public class GivenASyntaxInspectorInspectingBrokenCode
		{
			[TestCase(@"private void ApplicationInitializationStartup()
        {
            if (applicationInitTask == null)
            {
				applicationInitTask = Task.Factory.StartNew(() => SingleApplicationHostControl.Instance);
				applicationInitTask.Wait();
            }
        }", typeof(ImmediateTaskWaitRule))]
			[TestCase(@"private void SomeMethod()
        {
			do
			{
				if (DateTime.UtcNow.Millisecond == 500)
				{
					return true;
				}

				Thread.Sleep(100);
			}
			while (true)
		}", typeof(DoLoopSleepErrorRule))]
			[TestCase(@"private void SomeMethod()
        {
			while(true)
			{
				if (DateTime.UtcNow.Millisecond == 500)
				{
					return true;
				}

				Thread.Sleep(100);
			}
		}", typeof(WhileLoopSleepErrorRule))]
			[TestCase(@"private void SomeMethod()
        {
			while(true)
			{
			}
		}", typeof(EmptyWhileErrorRule))]
			[TestCase(@"private void SomeMethod()
        {
			do
			{
			}
			while(true)
		}", typeof(EmptyDoErrorRule))]
			[TestCase(@"private void SomeMethod()
        {
			public IServiceLocator ServiceLocator { get; set; }
		}", typeof(LeakingServiceLocatorRule))]
			[TestCase(@"private void SomeMethod()
        {
			var value = ServiceLocator.Current.Resolve<IMarkerInterface>();
		}", typeof(ServiceLocatorInvocationRule))]
			[TestCase(@"[TestMethod]
private void SomeMethod()
        {
			var value = ServiceLocator.Current.Resolve<IMarkerInterface>();
		}", typeof(ServiceLocatorInvocationInTestRule))]
			[TestCase(@"private void SomeMethod()
        {
			dynamic myVariable = 2;
		}", typeof(DynamicVariableRule))]
			[TestCase(@"private void SomeMethod()
        {
			var unityContainer = ServiceLocator.Current.Resolve<IUnityContainer>();
		}", typeof(ServiceLocatorResolvesContainerRule))]
			[TestCase(@"private void SomeMethod()
        {
			activity.ArchiveSettings.NetworkLocation = @""c:\"";
		}", typeof(DiskLocationDependencyRule))]
			[TestCase(@"private void SomeMethod()
        {
			var file = File.Create(""c:\blah.txt"");
		}", typeof(FileClassDependency))]
			[TestCase(@"private void SomeMethod()
        {
			var file = Directory.GetFiles(""c:\"");
		}", typeof(DirectoryClassDependency))]
			[TestCase(@"[TestMethod]
private void SomeMethod()
        {
			Assert.IsTrue(true);
			Assert.IsFalse(false);
		}", typeof(MultipleAssertsInTestErrorRule))]
			[TestCase(@"[TestMethod]
[ExpectedException(typeof(Exception))]
private void SomeMethod()
        {
			Assert.IsTrue(true);
			throw new Exception();
		}", typeof(MultipleAssertsInTestErrorRule))]
			[TestCase(@"[TestMethod]
[ExpectedException(typeof(Exception))]
private void SomeMethod()
        {
		}", typeof(EmptyTestRule))]
			[TestCase(@"[TestMethod]
[ExpectedException(typeof(Exception))]
private void SomeMethod()
        {
			// A comment
		}", typeof(EmptyTestRule))]
			[TestCase(@"[TestMethod]
private void SomeMethod()
        {
			someMock.Verify(x => x(a), Times.Once());
			anotherMock.Verify(x => x(a), Times.Once());
		}", typeof(MultipleAssertsInTestErrorRule))]
			[TestCase(@"private void SomeMethod()
        {
			var name = MethodBase.GetCurrentMethod().Name;
		}", typeof(ReflectionToResolveMethodNameRule))]
			[TestCase(@"private void SomeMethod()
        {
			var time = DateTime.Now;
		}", typeof(LocalTimeCreationRule))]
			[TestCase(@"private void SomeMethod()
        {
			goto SomeLabel;

			switch(x){
				case ""a"": return;
			}
		}", typeof(GotoStatementErrorRule))]
			[TestCase(@"private void SomeMethod(object x)
        {
			var time = x == someMethod() && a == b || b == c;
			if(x < y){
				if(x < y/2) {
					switch(x.Value){
						case ""a"":
						case ""b"":
						case ""c"":
						case ""d"":
						case ""e"":
							y = 2;
					}
				}
			}
		}", typeof(TooHighCyclomaticComplexityRule))]
			[TestCase(@"private void SomeMethod(MyClass x)
        {
			var value = FirstLevel.SecondLevel.ThirdLevelMethod();
		}", typeof(LawOfDemeterViolationRule))]
			[TestCase(@"private void SomeMethod(MyClass x)
        {
			object value = null;
		}", typeof(TypeObfuscationRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				activity.ArchiveSettings.NetworkLocation = @""c:\"";
			}
		}", typeof(PublicInterfaceImplementationWarningRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				if(x > 1)
				{
					return;
				}

				return;
			}
		}", typeof(MultipleReturnStatementsErrorRule))]
			[TestCase(@"public class InnerClass
		{
			public DomainStorage Storage
			{
				get { return null; }
			}
		}", typeof(LeakingDomainStorageRule))]
			[TestCase(@"public class InnerClass
		{
			public ISession Session
			{
				get { return null; }
			}
		}", typeof(LeakingSessionRule))]
			[TestCase(@"public class InnerClass
		{
			public UnityContainer Container
			{
				get { return null; }
			}
		}", typeof(LeakingUnityContainerRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = a > 1 ? ""a"" : ""b"";
			}
		}", typeof(ConditionalExpressionErrorRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = a ?? b;
			}
		}", typeof(CoalesceExpressionErrorRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private object x = new object();
			private void SomeMethod()
			{
				Guard.Against(x == null);
			}
		}", typeof(GuardClauseInMethodWithoutParametersRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private object x = new object();
			private void SomeMethod()
			{
				Guard.Against(x == null);
			}
		}", typeof(GuardClauseInNonPublicMethodRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = GetValue();
			}
		}", typeof(VarDeclarationForNewVariableErrorRule))]
			[TestCase(@"public class InnerClass
		{
			public void BeginSomeBeginMethod()
			{
				var x = GetValue();
			}

			public void EndSomeEndMethod()
			{
				var x = GetValue();
			}
		}", typeof(BeginEndPairRule))]
			[TestCase(@"public class InnerClass
		{
			public void OpenSomeOpenMethod()
			{
				var x = GetValue();
			}

			public void CloseSomeCloseMethod()
			{
				var x = GetValue();
			}
		}", typeof(OpenClosePairRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			public const string SomeValue = ""Something"";
		}", typeof(NoPublicConstantRule))]
			[TestCase(@"public class InnerClass
		{
			private int field = 0;
			public void AssignMethod(int field)
			{
				this.field = field;
			}
		}", typeof(VariableNameShouldNotMatchFieldNameRule))]
			public void SyntaxDetectionTest(string code, Type evaluatorType)
			{
				var task = PerformInspection(code, evaluatorType);
				var count = task.Result.Count();

				Assert.AreEqual(1, count);
			}
		}

		public class GivenASyntaxInspectorInspectingNonBrokenCode
		{
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = new object();
			}
		}", typeof(VarDeclarationForNewVariableErrorRule))]
			[TestCase("public abstract void DoSomething();", typeof(TooHighCyclomaticComplexityRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				while(true)
				{
				}
			}
		}", typeof(WhileLoopSleepErrorRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				do
				{
				}
				while(true);
			}
		}", typeof(DoLoopSleepErrorRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				do
				{
					AnotherMethod();
				}
				while(true);
			}
		}", typeof(EmptyDoErrorRule))]
			[TestCase(@"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = 1;
			}
		}", typeof(TypeObfuscationRule))]
			[TestCase(@"public class InnerClass
		{
			public void BeginSomeMethod()
			{
				var x = GetValue();
			}

			public void EndSomeMethod()
			{
				var x = GetValue();
			}
		}", typeof(BeginEndPairRule))]
			[TestCase(@"public class InnerClass
		{
			public void OpenSomeMethod()
			{
				var x = GetValue();
			}

			public void CloseSomeMethod()
			{
				var x = GetValue();
			}
		}", typeof(OpenClosePairRule))]
			[TestCase(@"public class InnerClass
		{
			private int field = 0;
			public void AssignMethod(int value)
			{
				this.field = value;
			}
		}", typeof(VariableNameShouldNotMatchFieldNameRule))]
			public void NegativeTest(string code, Type evaluatorType)
			{
				var task = PerformInspection(code, evaluatorType);
				task.Wait();
				Assert.IsEmpty(task.Result);
			}
		}
	}
}
