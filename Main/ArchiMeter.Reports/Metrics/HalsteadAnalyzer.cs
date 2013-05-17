namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Core.Metrics;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class HalsteadAnalyzer : SyntaxWalker
	{
		// Fields
		private IHalsteadMetrics _metrics;

		// Methods
		public HalsteadAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public IHalsteadMetrics Calculate(MemberNode node)
		{
			BlockSyntax syntax = MemberBodySelector.FindBody(node);
			if (syntax != null)
			{
				this.Visit(syntax);
				return _metrics;
			}
			if (this.CalculateGenericPropertyMetrics(node))
			{
				return _metrics;
			}
			return new HalsteadMetrics(0, 0, 0, 0);
		}

		private bool CalculateGenericPropertyMetrics(MemberNode node)
		{
			var syntaxNode = node.SyntaxNode as PropertyDeclarationSyntax;
			if (syntaxNode != null && MemberBodySelector.FindBody(node) == null)
			{
				bool flag = syntaxNode.Modifiers.Any(x => x.EquivalentTo(Syntax.Token(SyntaxKind.StaticKeyword)));
				switch (node.Kind)
				{


				}
			}
			return false;
		}

		private static IDictionary<SyntaxKind, IList<string>> ParseTokens(IEnumerable<SyntaxToken> tokens, IEnumerable<SyntaxKind> filter)
		{
			IDictionary<SyntaxKind, IList<string>> dictionary = new Dictionary<SyntaxKind, IList<string>>();
			foreach (SyntaxToken token in tokens)
			{
				SyntaxKind kind = token.Kind;
				if (filter.Any(x => x == kind))
				{
					IList<string> list;
					string valueText = token.ValueText;
					if (!dictionary.TryGetValue(kind, out list))
					{
						dictionary[kind] = new List<string>();
						list = dictionary[kind];
					}
					list.Add(valueText);
				}
			}
			return dictionary;
		}

		public override void VisitBlock(BlockSyntax node)
		{
			base.VisitBlock(node);
			IEnumerable<SyntaxToken> tokens = node.DescendantTokens().ToList();
			IDictionary<SyntaxKind, IList<string>> dictionary = ParseTokens(tokens, HalsteadOperands.All);
			IDictionary<SyntaxKind, IList<string>> dictionary2 = ParseTokens(tokens, HalsteadOperators.All);

			HalsteadMetrics metrics = new HalsteadMetrics(
				numOperands: dictionary.Values.Sum(x => x.Count),
				numOperators: dictionary2.Values.Sum(x => x.Count),
				numUniqueOperands: dictionary.Values.SelectMany(x => x).Distinct().Count(),
				numUniqueOperators: dictionary2.Values.SelectMany(x => x).Distinct().Count());
			_metrics = metrics;
		}
	}
}