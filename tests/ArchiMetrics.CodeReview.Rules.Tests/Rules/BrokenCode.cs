namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
    using System.Collections;
    using System.Collections.Generic;
    using CodeReview.Rules.Code;

    public class BrokenCode : IEnumerable<object[]>
    {
        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<object[]> GetEnumerator()
        {
            return GetCode().GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IEnumerable<object[]> GetCode()
        {
            yield return new object[]
            {
                @"public ParseClass()
{
	SomeEvent += SomeEventHandler;
}

public event EventHandler SomeEvent;

private void SomeEventHandler(object sender, EventArgs e)
{
	throw new NotImplementedException();
}",
                typeof(MissingEventHandlerDetachmentRule)
            };
            yield return new object[] { @"public class SomeClass : IDisposable { public void Dispose(){ }}", typeof(IncorrectDisposableImplementation) };
            yield return new object[]
            {
                @"private void ApplicationInitializationStartup()
		{
			if (applicationInitTask == null)
			{
				applicationInitTask = Task.Factory.StartNew(() => SingleApplicationHostControl.Instance);
				applicationInitTask.Wait();
			}
		}",
                typeof(ImmediateTaskWaitRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
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
		}",
                typeof(DoLoopSleepErrorRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			while(true)
			{
				if (DateTime.UtcNow.Millisecond == 500)
				{
					return true;
				}

				Thread.Sleep(100);
			}
		}",
                typeof(WhileLoopSleepErrorRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			while(true)
			{
			}
		}",
                typeof(EmptyWhileErrorRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			throw new NotImplementedException();
		}",
                typeof(NoNotImplementedExceptionRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			do
			{
			}
			while(true)
		}",
                typeof(EmptyDoErrorRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			public IServiceLocator ServiceLocator { get; set; }
		}",
                typeof(LeakingServiceLocatorRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			var value = ServiceLocator.Current.Resolve<IMarkerInterface>();
		}",
                typeof(ServiceLocatorInvocationRule)
            };
            yield return new object[]
            {
                @"[TestMethod]
private void SomeMethod()
		{
			var value = ServiceLocator.Current.Resolve<IMarkerInterface>();
		}",
                typeof(ServiceLocatorInvocationInTestRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			dynamic myVariable = 2;
		}",
                typeof(DynamicVariableRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			var unityContainer = ServiceLocator.Current.Resolve<IUnityContainer>();
		}",
                typeof(ServiceLocatorResolvesContainerRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			activity.ArchiveSettings.NetworkLocation = @""c:\"";
		}",
                typeof(DiskLocationDependencyRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			var file = File.Create(""c:\blah.txt"");
		}",
                typeof(FileClassDependency)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			var file = Directory.GetFiles(""c:\"");
		}",
                typeof(DirectoryClassDependency)
            };
            yield return new object[]
            {
                @"[TestMethod]
private void SomeMethod()
		{
			Assert.True(true);
			Assert.IsFalse(false);
		}",
                typeof(MultipleAssertsInTestErrorRule)
            };
            yield return new object[]
            {
                @"[TestMethod]
[ExpectedException(typeof(Exception))]
private void SomeMethod()
		{
			Assert.True(true);
			throw new Exception();
		}",
                typeof(MultipleAssertsInTestErrorRule)
            };
            yield return new object[]
            {
                @"[TestMethod]
[ExpectedException(typeof(Exception))]
private void SomeMethod()
		{
		}",
                typeof(EmptyTestRule)
            };
            yield return new object[]
            {
                @"[TestMethod]
[ExpectedException(typeof(Exception))]
private void SomeMethod()
		{
			// A comment
		}",
                typeof(EmptyTestRule)
            };
            yield return new object[]
            {
                @"[TestMethod]
private void SomeMethod()
		{
			someMock.Verify(x => x(a), Times.Once();
			anotherMock.Verify(x => x(a), Times.Once();
		}",
                typeof(MultipleAssertsInTestErrorRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			var name = MethodBase.GetCurrentMethod().Name;
		}",
                typeof(ReflectionToResolveMethodNameRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			var time = DateTime.Now;
		}",
                typeof(LocalTimeCreationRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
		{
			goto SomeLabel;

			switch(x){
				case ""a"": return;
			}
		}",
                typeof(GotoStatementErrorRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod(object x)
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
		}",
                typeof(TooHighCyclomaticComplexityRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod(MyClass x)
		{
			object value = null;
		}",
                typeof(TypeObfuscationRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				activity.ArchiveSettings.NetworkLocation = @""c:\"";
			}
		}",
                typeof(PublicInterfaceImplementationWarningRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				if(x > 1)
				{
					return;
				}

				return;
			}
		}",
                typeof(MultipleReturnStatementsErrorRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			public DomainStorage Storage
			{
				get { return null; }
			}
		}",
                typeof(LeakingDomainStorageRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			public ISession Session
			{
				get { return null; }
			}
		}",
                typeof(LeakingSessionRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			public UnityContainer Container
			{
				get { return null; }
			}
		}",
                typeof(LeakingUnityContainerRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = a > 1 ? ""a"" : ""b"";
			}
		}",
                typeof(ConditionalExpressionErrorRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = a ?? b;
			}
		}",
                typeof(CoalesceExpressionErrorRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private object x = new object();
			private void SomeMethod()
			{
				Guard.Against(x == null);
			}
		}",
                typeof(GuardClauseInMethodWithoutParametersRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private object x = new object();
			private void SomeMethod()
			{
				Guard.Against(x == null);
			}
		}",
                typeof(GuardClauseInNonPublicMethodRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = GetValue();
			}
		}",
                typeof(VarDeclarationForNewVariableErrorRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			public void BeginSomeBeginMethod()
			{
				var x = GetValue();
			}

			public void EndSomeEndMethod()
			{
				var x = GetValue();
			}
		}",
                typeof(BeginEndPairRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			public void OpenSomeOpenMethod()
			{
				var x = GetValue();
			}

			public void CloseSomeCloseMethod()
			{
				var x = GetValue();
			}
		}",
                typeof(OpenClosePairRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			public const string SomeValue = ""Something"";
		}",
                typeof(NoPublicConstantRule)
            };
            yield return new object[]
            {
                @"private void SomeMethod()
			{
				try
				{
					Int32.Parse (""Broken!"");
				} 
				catch (Exception ex)
				{
					throw ex;
				}
			}",
                typeof(DoNotDestroyStackTraceRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			private int field = 0;
			public void AssignMethod(int field)
			{
				this.field = field;
			}
		}",
                typeof(VariableNameShouldNotMatchFieldNameRule)
            };
            yield return new object[]
            {
                @"private void MyMethod(int x)
{
	if(DateTime.Now.Millisecond == 100)
	{
		switch(x)
		{
			case 1:
			case 2:
				{
					if(x == 1)
					{
						Console.WriteLine(""Hello"");
					}
					else
					{
						Console.WriteLine(""World"");
					}
				}
				break;
			default:
				break;
		}
	}
}",
                typeof(MethodTooDeepNestingRule)
            };
            yield return new object[]
            {
                @"public int MyProperty
{
	get
	{
		if(DateTime.Now.Millisecond == 100)
		{
			switch(value)
			{
				case 1:
				case 2:
					{
						if(value == 1)
						{
							return 10
						}
						else
						{
							return 3
						}
					}
					break;
				default:
					break;
			}
		}
	}
}",
                typeof(GetPropertyTooDeepNestingRule)
            };
            yield return new object[]
            {
                @"int _field;
public int MyProperty
{
	set
	{
		if(DateTime.Now.Millisecond == 100)
		{
			switch(value)
			{
				case 1:
				case 2:
					{
						if(value == 1)
						{
							_field = 10
						}
						else
						{
							_field = 3
						}
					}
					break;
				default:
					break;
			}
		}
	}
}",
                typeof(SetPropertyTooDeepNestingRule)
            };
        }
    }
}