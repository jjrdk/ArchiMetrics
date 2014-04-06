// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HalsteadAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HalsteadAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class HalsteadAnalyzer : SyntaxWalker
	{
		private IHalsteadMetrics _metrics = new HalsteadMetrics(0, 0, 0, 0);

		public HalsteadAnalyzer()
			: base(SyntaxWalkerDepth.Node)
		{
		}

		public IHalsteadMetrics Calculate(SyntaxNode syntax)
		{
			if (syntax != null)
			{
				Visit(syntax);
				return _metrics;
			}

			return _metrics;
		}

		/// <summary>
		/// Called when the walker visits a node.  This method may be overridden if subclasses want to handle the node.  Overrides should call back into this base method if they want the children of this node to be visited.
		/// </summary>
		/// <param name="node">The current node that the walker is visiting.</param>
		public override void Visit(SyntaxNode node)
		{
			var blockSyntax = node as BlockSyntax;
			if (blockSyntax != null)
			{
				VisitBlock(blockSyntax);
			}
			base.Visit(node);
		}

		public void VisitBlock(BlockSyntax node)
		{
			var tokens = node.DescendantTokens().ToList();
			var dictionary = ParseTokens(tokens, Operands.All);
			var dictionary2 = ParseTokens(tokens, Operators.All);
			var metrics = new HalsteadMetrics(
				numOperands: dictionary.Values.Sum(x => x.Count),
				numUniqueOperands: dictionary.Values.SelectMany(x => x).Distinct().Count(),
				numOperators: dictionary2.Values.Sum(x => x.Count),
				numUniqueOperators: dictionary2.Values.SelectMany(x => x).Distinct().Count());
			_metrics = metrics;
		}

		private static IDictionary<SyntaxKind, IList<string>> ParseTokens(IEnumerable<SyntaxToken> tokens, IEnumerable<SyntaxKind> filter)
		{
			IDictionary<SyntaxKind, IList<string>> dictionary = new Dictionary<SyntaxKind, IList<string>>();
			foreach (var token in tokens)
			{
				var kind = token.CSharpKind();
				if (filter.Any(x => x == kind))
				{
					IList<string> list;
					var valueText = token.ValueText;
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
	}
}
