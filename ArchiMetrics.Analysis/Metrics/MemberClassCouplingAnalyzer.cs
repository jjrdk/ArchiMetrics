// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberClassCouplingAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MemberClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		private readonly List<IMethodSymbol> _calledMethods;
		private readonly List<IPropertySymbol> _calledProperties;
		private readonly Dictionary<MemberKind, Action<SyntaxNode>> _classCouplingActions;
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
			_classCouplingActions = new Dictionary<MemberKind, Action<SyntaxNode>>
				                        {
					                        { MemberKind.Method, x => CalculateMethodClassCoupling((MethodDeclarationSyntax)x) }, 
					                        { MemberKind.Constructor, x => CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x) }, 
					                        { MemberKind.Destructor, x => CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x) }, 
					                        { MemberKind.GetProperty, x => CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.GetAccessorDeclaration) }, 
					                        { MemberKind.SetProperty, x => CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.SetAccessorDeclaration) }, 
					                        { MemberKind.AddEventHandler, x => CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.AddAccessorDeclaration) }, 
					                        { MemberKind.RemoveEventHandler, x => CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.RemoveAccessorDeclaration) }
				                        };
		}

		public IEnumerable<TypeCoupling> Calculate(MemberNode memberNode)
		{
			Action<SyntaxNode> action;
			var syntaxNode = (SyntaxNode)memberNode.SyntaxNode;
			if (_classCouplingActions.TryGetValue(memberNode.Kind, out action))
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
			CollectMemberCouplings(syntax.Body);
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
