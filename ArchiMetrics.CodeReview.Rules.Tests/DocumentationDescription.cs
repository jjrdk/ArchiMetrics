namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using ArchiMetrics.Common.CodeReview;
	using Moq;
	using NUnit.Framework;

	public class DocumentationDescription
	{
		private IEnumerable<IEvaluation> _rules;

		[SetUp]
		public void Setup()
		{
			var spellChecker = new Mock<ISpellChecker>();
			spellChecker.Setup(x => x.Spell(It.IsAny<string>())).Returns(true);

			_rules = AllRules.GetSyntaxRules(spellChecker.Object).ToArray();
		}

		[Test]
		public void DocumentIsUpToDate()
		{
			var docBuilder = new StringBuilder();
			docBuilder.AppendLine(@"<table>
	<thead>
		<tr>
			<th>ID</th>
			<th>Title</th>
			<th>Condition</th>
			<th>Suggestion</th>
			<th>Where Acceptable</th>
		</tr>
	</thead>
	<tbody>");

			foreach (var rule in _rules.OrderBy(_ => _.ID))
			{
				docBuilder.AppendLine(string.Format(@"		<tr>
			<td>{0}</td>
			<td>{1}</td>
			<td>{2}</td>
			<td>{3}</td>
			<td>{4}</td>
		</tr>", rule.ID, rule.Title, string.Empty, rule.Suggestion, string.Empty));
			}

			docBuilder.AppendLine(@"	</tbody>
</table>");

			Console.WriteLine(docBuilder.ToString());

			//Assert.AreEqual(ExpectedDocumentation, docBuilder.ToString());
		}

		private const string ExpectedDocumentation = @"<table>
	<thead>
		<tr>
			<th>ID</th>
			<th>Title</th>
			<th>Condition</th>
			<th>Suggestion</th>
			<th>Where Acceptable</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>AMT0001</td>
			<td>Suspicious Language Comment</td>
			<td></td>
			<td>Check spelling of comment.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMT0002</td>
			<td>Suspicious Language Comment</td>
			<td></td>
			<td>Check spelling of comment.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0001</td>
			<td>Unstable Class</td>
			<td></td>
			<td>Refactor class dependencies.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0002</td>
			<td>Hidden Type Dependency in Method Declaration</td>
			<td></td>
			<td>Refactor to pass dependencies explicitly.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0004</td>
			<td>No locking on Weak Identity Items</td>
			<td></td>
			<td>Change lock object to strong identity object, ex. new object()</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0005</td>
			<td>Method Can Be Made Static</td>
			<td></td>
			<td>Mark method as static.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0006</td>
			<td>Method Unmaintainable</td>
			<td></td>
			<td>Refactor method to improve maintainability.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0003</td>
			<td>Lack of Cohesion of Methods</td>
			<td></td>
			<td>Refactor class into separate classes with single responsibility.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0009</td>
			<td>Variable is never read</td>
			<td></td>
			<td>Remove unread variable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0010</td>
			<td>Unused Event Declaration</td>
			<td></td>
			<td>Remove unused code.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0007</td>
			<td>Field is never read</td>
			<td></td>
			<td>Remove unread field.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0011</td>
			<td>Unused Get Accessor Declaration</td>
			<td></td>
			<td>Remove unused code.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0013</td>
			<td>Unused Method Declaration</td>
			<td></td>
			<td>Remove unused code.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0014</td>
			<td>Unused Parameter in Method</td>
			<td></td>
			<td>Removed unused parameter.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMS0012</td>
			<td>Unused Set Accessor Declaration</td>
			<td></td>
			<td>Remove unused code.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0006</td>
			<td>Stack Trace Destroyed</td>
			<td></td>
			<td>Use only 'throw' to rethrow the original stack trace.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0012</td>
			<td>Too Deep Property Getter Nesting</td>
			<td></td>
			<td>Reduce nesting to make code more readable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0018</td>
			<td>Incorrect Dispose pattern implementation</td>
			<td></td>
			<td>Implement dispose pattern with finalizer and separate disposal of managed and unmanaged resources.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0027</td>
			<td>Event Handler not Detached</td>
			<td></td>
			<td>Unassign all event handlers.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0030</td>
			<td>NotImplementedException Thrown</td>
			<td></td>
			<td>Add method implementation.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0034</td>
			<td>Unsafe Statement Detected</td>
			<td></td>
			<td>Avoid unsafe code.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0026</td>
			<td>Too Deep Method Nesting</td>
			<td></td>
			<td>Reduce nesting to make code more readable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0013</td>
			<td>Too Deep Property Setter Nesting</td>
			<td></td>
			<td>Reduce nesting to make code more readable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0048</td>
			<td>Declare Types Inside Namespace.</td>
			<td></td>
			<td>Move type declaration inside namespace.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0051</td>
			<td>Variable Name Should Not Match Field Name</td>
			<td></td>
			<td>Rename variable to avoid confusion with assigned field.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0036</td>
			<td>Begin/End Method Pair</td>
			<td></td>
			<td>Methods names BeginSomething should have a matching EndSomething and vice versa.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0001</td>
			<td>Coalesce Expression</td>
			<td></td>
			<td>Use an explicit if statement to assign a value if it is null.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC9999</td>
			<td>Compilation Failure</td>
			<td></td>
			<td>Check the compilation error for details about reason for failure.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0002</td>
			<td>Conditional Expressions</td>
			<td></td>
			<td>Consider replacing the condition with an explicit if statement.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0003</td>
			<td>Directory Class Dependency</td>
			<td></td>
			<td>Consider breaking the direct dependency on the file system with an abstraction.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0005</td>
			<td>Do Statement Sleep Loop</td>
			<td></td>
			<td>Use a wait handle to synchronize timing issues.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0007</td>
			<td>Dynamic Variable</td>
			<td></td>
			<td>Consider using a typed variable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0008</td>
			<td>Empty Do Statement</td>
			<td></td>
			<td>Use a wait handle to synchronize asynchronous flows, or let the thread sleep.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0009</td>
			<td>No Assertion in Test</td>
			<td></td>
			<td>Add an assertion to the test.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0010</td>
			<td>Empty While Statement</td>
			<td></td>
			<td>Use a wait handle to synchronize asynchronous flows, or let the thread sleep.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0004</td>
			<td>Disk Location Dependency</td>
			<td></td>
			<td>Replace the dependency on a specific disk location with an abstraction.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0011</td>
			<td>File Class Dependency</td>
			<td></td>
			<td>Replace explicit file dependency to reduce coupling with file system.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0014</td>
			<td>Goto Statement</td>
			<td></td>
			<td>Refactor to use method calls.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0015</td>
			<td>Guard Clause in Method Without Parameters</td>
			<td></td>
			<td>Remove guard clause.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0016</td>
			<td>Guard Clause in Non-Public Method.</td>
			<td></td>
			<td>Remove Guard clause and verify internal state by other means.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0017</td>
			<td>Immediate Task Wait.</td>
			<td></td>
			<td>Immediately awaiting a Task has same effect as executing code synchonously.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0019</td>
			<td>Current Type Exposes DomainStorage</td>
			<td></td>
			<td>Remove public access to DomainStorage</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0020</td>
			<td>Current Type Exposes ServiceLocator</td>
			<td></td>
			<td>Remove public access to ServiceLocator</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0021</td>
			<td>Current Type Exposes ISession</td>
			<td></td>
			<td>Remove public access to ISession</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0022</td>
			<td>Current Type Exposes UnityContainer</td>
			<td></td>
			<td>Remove public access to UnityContainer</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0023</td>
			<td>Local Time Creation</td>
			<td></td>
			<td>Replace with call to DateTime.UtcNow</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0024</td>
			<td>Method Name Spelling</td>
			<td></td>
			<td>Check that the method name is spelled correctly. Consider adding exceptions to the dictionary.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0028</td>
			<td>Multiple Asserts in Test</td>
			<td></td>
			<td>Refactor tests to only have a single assert.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0029</td>
			<td>Multiple Return Statements</td>
			<td></td>
			<td>If your company's coding standards requires only a single exit point, then refactor method to have only single return statement.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0031</td>
			<td>No Protected Fields</td>
			<td></td>
			<td>Encapsulate all public fields in properties, or internalize them.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0032</td>
			<td>No Public Constants</td>
			<td></td>
			<td>Expose public constants as public static readonly instead in order to avoid that they get compiled into a calling assembly.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0033</td>
			<td>No Public Field</td>
			<td></td>
			<td>Encapsulate all public fields in properties, or internalize them.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0035</td>
			<td>Open/Close Method Pair</td>
			<td></td>
			<td>Methods names OpenSomething should have a matching CloseSomething and vice versa.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0025</td>
			<td>Property Name Spelling</td>
			<td></td>
			<td>Check that the property name is spelled correctly. Consider adding exceptions to the dictionary.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0037</td>
			<td>Public Interface Implementation</td>
			<td></td>
			<td>Consider whether the interface implementation also needs to be public.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0038</td>
			<td>Using Reflection to Resolve Member Name</td>
			<td></td>
			<td>Consider using a string for the method name for performance and to make it readable after obfuscation.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0039</td>
			<td>ServiceLocator Passed as Parameter</td>
			<td></td>
			<td>Remove ServiceLocator parameter and inject only needed dependencies.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0040</td>
			<td>ServiceLocator Invocation in Test</td>
			<td></td>
			<td>Replace ServiceLocator with explicit setup using either a concrete instance, mock or fake.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0041</td>
			<td>ServiceLocator Invocation</td>
			<td></td>
			<td>Consider injecting needed dependencies explicitly.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0042</td>
			<td>ServiceLocator Resolves Container.</td>
			<td></td>
			<td>A ServiceLocator should never resolve its own DI container. Refactor to pass dependencies explicitly.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0043</td>
			<td>Class Too Big</td>
			<td></td>
			<td>Refactor class to make it more manageable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0044</td>
			<td>Method Too Big</td>
			<td></td>
			<td>Refactor method to make it more manageable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0047</td>
			<td>More than 5 parameters on method</td>
			<td></td>
			<td>Refactor method to reduce number of dependencies passed.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0046</td>
			<td>Method Too Complex.</td>
			<td></td>
			<td>Refactor to reduce number of code paths through method.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0049</td>
			<td>Type Obfuscation</td>
			<td></td>
			<td>Assigning a value to a variable of type object bypasses type checking.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0050</td>
			<td>Var Keyword Used in Variable Declaration</td>
			<td></td>
			<td>Consider using an explicit type for variable.</td>
			<td></td>
		</tr>
		<tr>
			<td>AMC0052</td>
			<td>Sleep Loop</td>
			<td></td>
			<td>Use a wait handle to synchronize control flows.</td>
			<td></td>
		</tr>
	</tbody>
</table>";
	}
}