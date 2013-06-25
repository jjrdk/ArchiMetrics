// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberClassCouplingAnalyzer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberClassCouplingAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MemberClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		private readonly Dictionary<MemberKind, Action<SyntaxNode>> _classCouplingActions;
		private readonly Dictionary<CommonSymbolKind, Action<ISymbol>> _symbolActions;

		// Methods
		public MemberClassCouplingAnalyzer(ISemanticModel semanticModel)
			: base(semanticModel)
		{
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

			return GetCollectedTypesNames();
		}

		public override void VisitIdentifierName(IdentifierNameSyntax node)
		{
			base.VisitIdentifierName(node);
			CommonSymbolInfo symbolInfo = SemanticModel.GetSymbolInfo(node);
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
			TypeSyntax type = node.Type;
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
			BlockSyntax accessor = GetAccessor(syntax.AccessorList, kind);
			if (accessor != null)
			{
				Visit(accessor);
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
		}

		private void CalculatePropertyClassCoupling(PropertyDeclarationSyntax syntax, SyntaxKind kind)
		{
			FilterType(syntax.Type);
			BlockSyntax accessor = GetAccessor(syntax.AccessorList, kind);
			if (accessor != null)
			{
				Visit(accessor);
			}
		}
	}
}