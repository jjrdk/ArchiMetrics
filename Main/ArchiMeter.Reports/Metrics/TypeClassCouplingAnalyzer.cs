namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class TypeClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		// Methods
		public TypeClassCouplingAnalyzer(ISemanticModel semanticModel)
			: base(semanticModel)
		{
		}

		public IEnumerable<string> Calculate(TypeDeclarationSyntax typeNode)
		{
			SyntaxNode node = typeNode;
			this.Visit(node);
			return base.GetCollectedTypesNames();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			Func<TypeSyntax, CommonSymbolInfo> selector = null;
			base.VisitClassDeclaration(node);
			if (node.BaseList != null)
			{
				if (selector == null)
				{
					selector = x => base.SemanticModel.GetSymbolInfo(x, new CancellationToken());
				}
				NamedTypeSymbol symbol = (from x in node.BaseList.Types.Select<TypeSyntax, CommonSymbolInfo>(selector)
				                          where (x.Symbol != null) && (x.Symbol.Kind == CommonSymbolKind.NamedType)
				                          select x.Symbol).OfType<NamedTypeSymbol>().FirstOrDefault<NamedTypeSymbol>();
				if (symbol != null)
				{
					base.FilterTypeSymbol(symbol);
				}
			}
		}

		public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			base.VisitFieldDeclaration(node);
			base.FilterType(node.Declaration.Type);
		}
	}
}