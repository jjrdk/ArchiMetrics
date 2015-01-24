// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassCouplingAnalyzerBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal abstract class ClassCouplingAnalyzerBase : CSharpSyntaxWalker
	{
		private readonly SemanticModel _semanticModel;
		private readonly IDictionary<string, ITypeSymbol> _types;

		protected ClassCouplingAnalyzerBase(SemanticModel semanticModel)
			: base(SyntaxWalkerDepth.Node)
		{
			_types = new Dictionary<string, ITypeSymbol>();

			_semanticModel = semanticModel;
		}

		protected SemanticModel SemanticModel
		{
			get
			{
				return _semanticModel;
			}
		}

		protected void FilterType(TypeSyntax syntax)
		{
			if (syntax.IsKind(SyntaxKind.PredefinedType))
			{
				var symbolInfo = SemanticModel.GetSymbolInfo(syntax);
				if ((symbolInfo.Symbol != null) && (symbolInfo.Symbol.Kind == SymbolKind.NamedType))
				{
					var symbol = (ITypeSymbol)symbolInfo.Symbol;
					FilterTypeSymbol(symbol);
				}
			}
		}

		protected void FilterTypeSymbol(ITypeSymbol symbol)
		{
			switch (symbol.TypeKind)
			{
				case TypeKind.Class:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Interface:
					{
						var qualifiedName = symbol.GetQualifiedName().ToString();
						if (!_types.ContainsKey(qualifiedName))
						{
							_types.Add(qualifiedName, symbol);
						}

						break;
					}

				case TypeKind.Dynamic:
				case TypeKind.Error:
				case TypeKind.TypeParameter:
					break;

				default:
					return;
			}
		}

		protected IEnumerable<ITypeCoupling> GetCollectedTypesNames()
		{
			return GetCollectedTypesNames(new IPropertySymbol[0], new IMethodSymbol[0], new IEventSymbol[0]);
		}

		protected IEnumerable<ITypeCoupling> GetCollectedTypesNames(IEnumerable<IPropertySymbol> calledProperties, IEnumerable<IMethodSymbol> calledMethods, IEnumerable<IEventSymbol> usedEvents)
		{
			var memberCouplings = _types.Select(x => CresateTypeCoupling(calledProperties, calledMethods, usedEvents, x)).AsArray();
			var inheritedCouplings = _types
				.Select(x => x.Value)
				.SelectMany(GetInheritedTypeNames);
			var interfaces = _types.SelectMany(x => x.Value.AllInterfaces);
			var inheritedTypeCouplings = inheritedCouplings.Concat(interfaces)
				.Select(CreateTypeCoupling)
				.Except(memberCouplings);

			return memberCouplings.Concat(inheritedTypeCouplings);
		}

		private static TypeCoupling CresateTypeCoupling(
			IEnumerable<IPropertySymbol> calledProperties,
			IEnumerable<IMethodSymbol> calledMethods,
			IEnumerable<IEventSymbol> usedEvents,
			KeyValuePair<string, ITypeSymbol> x)
		{
			var typeSymbol = x.Value;
			var usedMethods =
				calledMethods.Where(m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString())
					.Select(m => m.ToDisplayString());
			var usedProperties =
				calledProperties.Where(m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString())
					.Select(m => m.ToDisplayString());
			var events =
				usedEvents.Where(m => m.ContainingType.ToDisplayString() == typeSymbol.ToDisplayString())
					.Select(m => m.ToDisplayString());

			return CreateTypeCoupling(typeSymbol, usedMethods, usedProperties, events);
		}

		private static TypeCoupling CreateTypeCoupling(ITypeSymbol typeSymbol)
		{
			return CreateTypeCoupling(typeSymbol, Enumerable.Empty<string>(), Enumerable.Empty<string>(), Enumerable.Empty<string>());
		}

		private static TypeCoupling CreateTypeCoupling(ITypeSymbol typeSymbol, IEnumerable<string> usedMethods, IEnumerable<string> usedProperties, IEnumerable<string> events)
		{
			var name = typeSymbol.IsAnonymousType ? typeSymbol.ToDisplayString() : typeSymbol.Name;

			var namespaceName = string.Join(".", GetFullNamespace(typeSymbol.ContainingNamespace));
			if (string.IsNullOrWhiteSpace(namespaceName))
			{
				namespaceName = "global";
			}

			var assemblyName = "Unknown";
			if (typeSymbol.ContainingAssembly != null)
			{
				assemblyName = typeSymbol.ContainingAssembly.Name;
			}

			return new TypeCoupling(name, namespaceName, assemblyName, usedMethods, usedProperties, events);
		}

		private static IEnumerable<ITypeSymbol> GetInheritedTypeNames(ITypeSymbol symbol)
		{
			if (symbol.BaseType == null)
			{
				yield break;
			}

			yield return symbol.BaseType;
			foreach (var name in GetInheritedTypeNames(symbol.BaseType))
			{
				yield return name;
			}
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
