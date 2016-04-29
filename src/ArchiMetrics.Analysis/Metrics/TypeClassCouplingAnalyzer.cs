// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeClassCouplingAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeClassCouplingAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class TypeClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		public TypeClassCouplingAnalyzer(SemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public IEnumerable<ITypeCoupling> Calculate(TypeDeclarationSyntax typeNode)
		{
			SyntaxNode node = typeNode;
			Visit(node);
			return GetCollectedTypesNames();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node);
			if (node.BaseList != null)
			{
				var symbol = node.BaseList.Types
								 .Select(x => SemanticModel.GetSymbolInfo(x))
								 .Where(x => (x.Symbol != null) && (x.Symbol.Kind == SymbolKind.NamedType))
								 .Select(x => x.Symbol)
								 .OfType<INamedTypeSymbol>()
								 .FirstOrDefault();
				if (symbol != null)
				{
					FilterTypeSymbol(symbol);
				}
			}
		}

		public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			base.VisitFieldDeclaration(node);
			FilterType(node.Declaration.Type);
		}
	}
}
