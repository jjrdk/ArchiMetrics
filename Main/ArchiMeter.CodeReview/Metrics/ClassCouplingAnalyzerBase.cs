// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassCouplingAnalyzerBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ClassCouplingAnalyzerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal abstract class ClassCouplingAnalyzerBase : SyntaxWalker
	{
		// Fields
		private readonly ISemanticModel _semanticModel;
		private readonly IDictionary<string, TypeSymbol> _types;

		// Methods
		protected ClassCouplingAnalyzerBase(ISemanticModel semanticModel)
			: base(SyntaxWalkerDepth.Node)
		{
			_types = new Dictionary<string, TypeSymbol>();

			_semanticModel = semanticModel;
		}

		protected ISemanticModel SemanticModel
		{
			get
			{
				return _semanticModel;
			}
		}

		protected void FilterType(TypeSyntax syntax)
		{
			if (syntax.Kind != SyntaxKind.PredefinedType)
			{
				CommonSymbolInfo symbolInfo = SemanticModel.GetSymbolInfo(syntax);
				if ((symbolInfo.Symbol != null) && (symbolInfo.Symbol.Kind == CommonSymbolKind.NamedType))
				{
					var symbol = (TypeSymbol)symbolInfo.Symbol;
					FilterTypeSymbol(symbol);
				}
			}
		}

		protected void FilterTypeSymbol(TypeSymbol symbol)
		{
			switch (symbol.TypeKind)
			{
				case TypeKind.Class:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Interface:
					{
						TypeSymbol symbol2;
						string qualifiedName = TypeNameResolver.GetQualifiedName(symbol);
						if (!_types.TryGetValue(qualifiedName, out symbol2))
						{
							_types[qualifiedName] = symbol;
						}

						break;
					}

				case TypeKind.DynamicType:
				case TypeKind.Error:
				case TypeKind.TypeParameter:
					break;

				default:
					return;
			}
		}

		protected IEnumerable<TypeCoupling> GetCollectedTypesNames()
		{
			return _types.Select(x =>
									 {
										 var typeSymbol = x.Value;
										 var ns = string.Join(".", GetFullNamespace(typeSymbol.ContainingNamespace));
										 return new TypeCoupling(typeSymbol.Name, ns, typeSymbol.ContainingAssembly.Name);
									 })
						 .ToArray();
		}

		private IEnumerable<string> GetFullNamespace(NamespaceSymbol namespaceSymbol)
		{
			if (namespaceSymbol.ContainingNamespace != null
				&& !namespaceSymbol.ContainingNamespace.IsGlobalNamespace)
			{
				foreach (var ns in GetFullNamespace(namespaceSymbol.ContainingNamespace))
				{
					yield return ns;
				}
			}

			yield return namespaceSymbol.Name;
		}
	}
}