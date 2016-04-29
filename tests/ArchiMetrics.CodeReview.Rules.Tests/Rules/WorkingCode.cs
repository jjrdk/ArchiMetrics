namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
    using System.Collections;
    using System.Collections.Generic;
    using CodeReview.Rules.Code;

    public class WorkingCode : IEnumerable<object[]>
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

        public static IEnumerable<object[]> GetCode()
        {
            yield return new object[]
            {
                @"public ParseClass()
{
	SomeEvent += SomeEventHandler;
	SomeEvent -= SomeEventHandler;
}

public event EventHandler SomeEvent;

private void SomeEventHandler(object sender, EventArgs e)
{
	throw new NotImplementedException();
}",
                typeof(MissingEventHandlerDetachmentRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : IDisposable
		{
			~InnerClass()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
			}

			protected void Dispose(bool isDisposing)
			{
			}
		}",
                typeof(IncorrectDisposableImplementation)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = new object();
			}
		}",
                typeof(VarDeclarationForNewVariableErrorRule)
            };
            yield return new object[] { "public abstract void DoSomething();", typeof(TooHighCyclomaticComplexityRule) };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				while(true)
				{
				}
			}
		}",
                typeof(WhileLoopSleepErrorRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				do
				{
				}
				while(true);
			}
		}",
                typeof(DoLoopSleepErrorRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				try
				{
					Int32.Parse (""Broken!"");
				}
				catch (Exception ex)
				{
					throw;
				}
			}
		}",
                typeof(DoNotDestroyStackTraceRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				do
				{
					AnotherMethod();
				}
				while(true);
			}
		}",
                typeof(EmptyDoErrorRule)
            };
            yield return new object[]
            {
                @"public class InnerClass : ICustomInterface
		{
			private void SomeMethod()
			{
				var x = 1;
			}
		}",
                typeof(TypeObfuscationRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			public void BeginSomeMethod()
			{
				var x = GetValue();
			}

			public void EndSomeMethod()
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
			public void OpenSomeMethod()
			{
				var x = GetValue();
			}

			public void CloseSomeMethod()
			{
				var x = GetValue();
			}
		}",
                typeof(OpenClosePairRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			private int field = 0;
			public void AssignMethod(int value)
			{
				this.field = value;
			}
		}",
                typeof(VariableNameShouldNotMatchFieldNameRule)
            };
            yield return new object[]
            {
                @"public class InnerClass
		{
			private int field = 0;
			public void AssignMethod(int value)
			{
				this.field = value;
			}
		}",
                typeof(MethodTooDeepNestingRule)
            };
        }
    }
}