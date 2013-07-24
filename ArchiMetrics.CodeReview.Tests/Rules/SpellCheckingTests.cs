namespace ArchiMetrics.CodeReview.Tests.Rules
{
	using System;
	using System.IO;
	using System.Linq;
	using Code;
	using Common;
	using Ionic.Zip;
	using NHunspell;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;
	using Trivia;

	public sealed class SpellCheckingTests
	{
		private SpellCheckingTests()
		{
		}

		private class ExemptWords : IKnownWordList
		{
			public bool IsExempt(string word)
			{
				return false;
			}
		}

		public class GivenAMethodNameSpellingRule
		{
			private MethodNameSpellingRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new MethodNameSpellingRule(new SpellChecker(), new ExemptWords());
			}

			[TestCase("SomMethod")]
			[TestCase("CalclateValue")]
			[TestCase("GetValu")]
			public void FindMispelledMethodNames(string methodName)
			{
				var method = SyntaxTree.ParseText(string.Format(@"public void {0}() {{ }}", methodName));
				var result = _rule.Evaluate(method.GetRoot()
					.ChildNodes()
					.OfType<MethodDeclarationSyntax>()
					.First());

				Assert.NotNull(result);
			}
		}

		public class GivenAMultiLineCommentLanguageRule
		{
			private MultiLineCommentLanguageRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new MultiLineCommentLanguageRule(new SpellChecker());
			}

			[TestCase("ASP.NET MVC is a .NET acronym.")]
			[TestCase("Donde esta la cerveza?")]
			[TestCase("Dette er ikke en engelsk kommentar.")]
			public void FindNonEnglishMultiLineComments(string comment)
			{
				var method = SyntaxTree.ParseText(
					string.Format(@"public void SomeMethod() {{
/* {0} */
}}", comment));
				var root = method.GetRoot().DescendantNodes().OfType<BlockSyntax>().First();
				var nodes = root
					.DescendantTrivia(descendIntoTrivia: true)
					.Where(t => t.Kind == SyntaxKind.MultiLineCommentTrivia)
					.ToArray();
				var result = _rule.Evaluate(nodes.First());

				Assert.NotNull(result);
			}

			[TestCase(".NET has syntactic sugar the iterator pattern.")]
			[TestCase("This comment is in English.")]
			public void AcceptsEnglishMultiLineComments(string comment)
			{
				var method = SyntaxTree.ParseText(
					string.Format(@"public void SomeMethod() {{
/* {0} */
}}", comment));
				var root = method.GetRoot().DescendantNodes().OfType<BlockSyntax>().First();
				var nodes = root
					.DescendantTrivia(descendIntoTrivia: true)
					.Where(t => t.Kind == SyntaxKind.MultiLineCommentTrivia)
					.ToArray();
				var result = _rule.Evaluate(nodes.First());

				Assert.Null(result);
			}
		}

		public class GivenASingleLineCommentLanguageRule
		{
			private SingleLineCommentLanguageRule _rule;

			[SetUp]
			public void Setup()
			{
				_rule = new SingleLineCommentLanguageRule(new SpellChecker());
			}

			[TestCase("Dette er ikke en engelsk kommentar.")]
			public void FindNonEnglishSingleLineComments(string comment)
			{
				var method = SyntaxTree.ParseText(
					string.Format(@"public void SomeMethod() {{
//{0}
}}", comment));
				var root = method.GetRoot().DescendantNodes().OfType<BlockSyntax>().First();
				var nodes = root
					.DescendantTrivia(descendIntoTrivia: true)
					.Where(t => t.Kind == SyntaxKind.SingleLineCommentTrivia)
					.ToArray();
				var result = _rule.Evaluate(nodes.First());

				Assert.NotNull(result);
			}
		}

		public class GivenASolutionInspectorWithCommentLanguageRules
		{
			private NodeInspector _inspector;

			[SetUp]
			public void Setup()
			{
				var spellChecker = new SpellChecker();
				_inspector = new NodeInspector(new IEvaluation[] { new SingleLineCommentLanguageRule(spellChecker), new MultiLineCommentLanguageRule(spellChecker) });
			}

			[TestCase("//Dette er ikke en engelsk kommentar.")]
			[TestCase("/* Dette er ikke en engelsk kommentar. */")]
			public void WhenInspectingCommentsThenDetectsSuspiciousLanguage(string comment)
			{
				var method = SyntaxTree.ParseText(
					string.Format(@"public void SomeMethod() {{
{0}
}}", comment));
				var root = method.GetRoot();

				var task = _inspector.Inspect(string.Empty, root, null, null);
				task.Wait();

				Assert.IsNotEmpty(task.Result);
			}
		}

		private class SpellChecker : ISpellChecker
		{
			private readonly Hunspell _speller;

			public SpellChecker()
			{
				using (var dictFile = ZipFile.Read(@"Dictionaries\dict-en.oxt"))
				{
					var affStream = new MemoryStream();
					var dicStream = new MemoryStream();
					dictFile.FirstOrDefault(z => z.FileName == "en_US.aff").Extract(affStream);
					dictFile.FirstOrDefault(z => z.FileName == "en_US.dic").Extract(dicStream);
					_speller = new Hunspell(affStream.ToArray(), dicStream.ToArray());
				}
			}

			public bool Spell(string word)
			{
				return _speller.Spell(word);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			~SpellChecker()
			{
				Dispose(false);
			}

			protected virtual void Dispose(bool isDisposing)
			{
				if (isDisposing)
				{
					_speller.Dispose(true);
				}
			}
		}
	}
}
