namespace ArchiMetrics.CodeReview.Rules.Tests
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using ArchiMetrics.Common;
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

			_rules = AllRules.GetSyntaxRules(spellChecker.Object).AsArray();
		}

		[Test]
        [Ignore]
		public void DocumentIsUpToDate()
		{
			var fullpath = Path.GetFullPath(@"..\..\..\RulesList.xml");
			var docText = File.ReadAllText(fullpath);
			var children =
				_rules.OrderBy(_ => _.ID)
					.Select(
						rule =>
						new XElement(
							"rule",
							new XElement("id", rule.ID),
							new XElement("title", rule.Title),
							new XElement("suggestion", rule.Suggestion)))
					.Cast<object>()
					.ToArray();
			var rules = new XElement("rules", children);
			var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rules);
			var currentText = "";
			using (var ms = new MemoryStream())
			{
				doc.Save(ms);
				ms.Flush();
				ms.Position = 0;
				using (var reader = new StreamReader(ms))
				{
					currentText = reader.ReadToEnd();
				}
			}

			Assert.AreEqual(docText, currentText);
		}
	}
}