namespace ArchiMeter.CodeReview.Tests.Rules
{
	using System.IO;
	using System.Linq;
	using CodeReview.Rules;
	using Common;
	using Ionic.Zip;
	using NHunspell;
	using NUnit.Framework;
	using Roslyn.Compilers.CSharp;

	public class SpellCheckingTests
	{
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

		private class SpellChecker : ISpellChecker
		{
			private readonly Hunspell _speller;

			public SpellChecker()
			{
				using (var dictFile = ZipFile.Read(@"Dictionaries\dict-en.oxt"))
				{
					var affStream = new MemoryStream();
					var dicStream = new MemoryStream();
					var entries = dictFile.Select(z => z.FileName)
						.ToArray();
					dictFile.FirstOrDefault(z => z.FileName == "en_US.aff")
						.Extract(affStream);
					dictFile.FirstOrDefault(z => z.FileName == "en_US.dic")
						.Extract(dicStream);
					_speller = new Hunspell(affStream.ToArray(), dicStream.ToArray());
				}
			}

			public bool Spell(string word)
			{
				return _speller.Spell(word);
			}
		}

		private class ExemptWords : IKnownWordList
		{
			public bool IsExempt(string word)
			{
				return false;
			}
		}
	}
}