// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberClassCouplingAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberClassCouplingAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using ArchiMetrics.Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MemberClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		private readonly List<IMethodSymbol> _calledMethods;
		private readonly List<IPropertySymbol> _calledProperties;
		private readonly Dictionary<SyntaxKind, Action<SyntaxNode>> _classCouplingActions;
		private readonly Dictionary<CommonSymbolKind, Action<ISymbol>> _symbolActions;
		private readonly List<IEventSymbol> _usedEvents;

		public MemberClassCouplingAnalyzer(ISemanticModel semanticModel)
			: base(semanticModel)
		{
			_calledMethods = new List<IMethodSymbol>();
			_calledProperties = new List<IPropertySymbol>();
			_usedEvents = new List<IEventSymbol>();
			_symbolActions = new Dictionary<CommonSymbolKind, Action<ISymbol>>
			                 {
				                 { CommonSymbolKind.NamedType, x => FilterTypeSymbol((TypeSymbol)x) }, 
				                 { CommonSymbolKind.Parameter, x => FilterTypeSymbol(((ParameterSymbol)x).Type) }, 
				                 { CommonSymbolKind.Method, x => FilterTypeSymbol(((MethodSymbol)x).ContainingType) }, 
				                 { CommonSymbolKind.Field, x => FilterTypeSymbol(((FieldSymbol)x).Type) }, 
				                 { CommonSymbolKind.Property, x => FilterTypeSymbol(((PropertySymbol)x).ContainingType) }, 
				                 { CommonSymbolKind.Event, x => FilterTypeSymbol(((EventSymbol)x).ContainingType) }
			                 };

			_classCouplingActions = new Dictionary<SyntaxKind, Action<SyntaxNode>>
			                        {
				                        { SyntaxKind.MethodDeclaration, x => CalculateMethodClassCoupling((MethodDeclarationSyntax)x) }, 
				                        { SyntaxKind.ConstructorDeclaration, x => CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x) }, 
				                        { SyntaxKind.DestructorDeclaration, x => CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x) }, 
				                        { SyntaxKind.GetAccessorDeclaration, x => CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.GetAccessorDeclaration) }, 
				                        { SyntaxKind.SetAccessorDeclaration, x => CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.SetAccessorDeclaration) }, 
				                        { SyntaxKind.AddAccessorDeclaration, x => CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.AddAccessorDeclaration) }, 
				                        { SyntaxKind.RemoveAccessorDeclaration, x => CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.RemoveAccessorDeclaration) }
			                        };
		}

		public IEnumerable<ITypeCoupling> Calculate(SyntaxNode syntaxNode)
		{
			Action<SyntaxNode> action;

			if (_classCouplingActions.TryGetValue(syntaxNode.Kind, out action))
			{
				action(syntaxNode);
			}

			return GetCollectedTypesNames(_calledProperties, _calledMethods, _usedEvents);
		}

		public override void VisitIdentifierName(IdentifierNameSyntax node)
		{
			base.VisitIdentifierName(node);
			var symbolInfo = SemanticModel.GetSymbolInfo(node);
			if (symbolInfo.Symbol != null)
			{
				Action<ISymbol> action;
				var symbol = symbolInfo.Symbol;
				if (_symbolActions.TryGetValue(symbol.Kind, out action))
				{
					action(symbol);
				}
			}
		}

		public override void VisitParameter(ParameterSyntax node)
		{
			base.VisitParameter(node);
			var type = node.Type;
			if (type != null)
			{
				FilterType(type);
			}
		}

		private static BlockSyntax GetAccessor(AccessorListSyntax accessorList, SyntaxKind kind)
		{
			return accessorList.Accessors.Single(x => x.Kind == kind).Body;
		}

		private void CalculateEventClassCoupling(EventDeclarationSyntax syntax, SyntaxKind kind)
		{
			FilterType(syntax.Type);
			var accessor = GetAccessor(syntax.AccessorList, kind);
			if (accessor != null)
			{
				Visit(accessor);
				CollectMemberCouplings(accessor);
			}
		}

		private void CalculateGenericMemberClassCoupling(MemberDeclarationSyntax syntax)
		{
			Visit(syntax);
		}

		private void CalculateMethodClassCoupling(MethodDeclarationSyntax syntax)
		{
			Visit(syntax);
			FilterType(syntax.ReturnType);
			if (syntax.Body != null)
			{
				CollectMemberCouplings(syntax.Body);
			}
		}

		private void CalculatePropertyClassCoupling(PropertyDeclarationSyntax syntax, SyntaxKind kind)
		{
			FilterType(syntax.Type);
			var accessor = GetAccessor(syntax.AccessorList, kind);
			if (accessor != null)
			{
				Visit(accessor);
				CollectMemberCouplings(accessor);
			}
		}

		private void CollectMemberCouplings(SyntaxNode syntax)
		{
			if (syntax == null)
			{
				return;
			}

			var methodCouplings = GetMemberCouplings<MemberAccessExpressionSyntax>(syntax)
				.Union(GetMemberCouplings<IdentifierNameSyntax>(syntax))
				.Where(x => x.Kind == CommonSymbolKind.Method || x.Kind == CommonSymbolKind.Property || x.Kind == CommonSymbolKind.Event)
				.ToArray();
			_calledMethods.AddRange(methodCouplings.Where(x => x.Kind == CommonSymbolKind.Method).Cast<IMethodSymbol>());
			_calledProperties.AddRange(methodCouplings.Where(x => x.Kind == CommonSymbolKind.Property).Cast<IPropertySymbol>());
			_usedEvents.AddRange(methodCouplings.Where(x => x.Kind == CommonSymbolKind.Event).Cast<IEventSymbol>());
		}

		private IEnumerable<ISymbol> GetMemberCouplings<T>(SyntaxNode block)
			where T : ExpressionSyntax
		{
			return block
				.DescendantNodes()
				.OfType<T>()
				.Select(r =>
						new
							{
								node = r, 
								model = SemanticModel
							})
				.Select(info => info.model.GetSymbolInfo(info.node).Symbol)
				.Where(x => x != null);
		}
	}
}
