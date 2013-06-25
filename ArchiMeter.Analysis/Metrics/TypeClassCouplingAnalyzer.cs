// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeClassCouplingAnalyzer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeClassCouplingAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class TypeClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		public TypeClassCouplingAnalyzer(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public IEnumerable<TypeCoupling> Calculate(TypeDeclarationSyntax typeNode)
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
								 .Where(x => (x.Symbol != null) && (x.Symbol.Kind == CommonSymbolKind.NamedType))
								 .Select(x => x.Symbol)
								 .OfType<NamedTypeSymbol>()
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