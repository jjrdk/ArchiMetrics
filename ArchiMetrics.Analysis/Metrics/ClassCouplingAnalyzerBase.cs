// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassCouplingAnalyzerBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ClassCouplingAnalyzerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal abstract class ClassCouplingAnalyzerBase : SyntaxWalker
	{
		private readonly ISemanticModel _semanticModel;
		private readonly IDictionary<string, ITypeSymbol> _types;

		protected ClassCouplingAnalyzerBase(ISemanticModel semanticModel)
			: base(SyntaxWalkerDepth.Node)
		{
			_types = new Dictionary<string, ITypeSymbol>();

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
				var symbolInfo = SemanticModel.GetSymbolInfo(syntax);
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
						ITypeSymbol symbol2;
						var qualifiedName = symbol.GetQualifiedName();
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
			return GetCollectedTypesNames(new IPropertySymbol[0], new IMethodSymbol[0], new IEventSymbol[0]);
		}

		protected IEnumerable<TypeCoupling> GetCollectedTypesNames(IEnumerable<IPropertySymbol> calledProperties, IEnumerable<IMethodSymbol> calledMethods, IEnumerable<IEventSymbol> usedEvents)
		{
			return _types.Select(x =>
				{
					var typeSymbol = x.Value;
					var ns = string.Join(".", GetFullNamespace(typeSymbol.ContainingNamespace));
					var usedMethods = calledMethods.Where(m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString()).Select(m => m.ToDisplayString());
					var usedProperties = calledProperties.Where(m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString()).Select(m => m.ToDisplayString());
					var events = usedEvents.Where(m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString()).Select(m => m.ToDisplayString());

					return new TypeCoupling(typeSymbol.Name, ns, typeSymbol.ContainingAssembly.Name, usedMethods, usedProperties, events);
				})
						 .ToArray();
		}

		private static IEnumerable<string> GetFullNamespace(INamespaceSymbol namespaceSymbol)
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
