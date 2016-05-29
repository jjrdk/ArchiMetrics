// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberClassCouplingAnalyzer.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
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
	using Common;
	using Common.Metrics;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class MemberClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		private readonly List<IMethodSymbol> _calledMethods;
		private readonly List<IPropertySymbol> _calledProperties;
		private readonly Dictionary<SyntaxKind, Action<SyntaxNode>> _classCouplingActions;
		private readonly Dictionary<SymbolKind, Action<ISymbol>> _symbolActions;
		private readonly List<IEventSymbol> _usedEvents;

		public MemberClassCouplingAnalyzer(SemanticModel semanticModel)
			: base(semanticModel)
		{
			_calledMethods = new List<IMethodSymbol>();
			_calledProperties = new List<IPropertySymbol>();
			_usedEvents = new List<IEventSymbol>();
			_symbolActions = new Dictionary<SymbolKind, Action<ISymbol>>
			                 {
				                 { SymbolKind.NamedType, x => FilterTypeSymbol((ITypeSymbol)x) }, 
				                 { SymbolKind.Parameter, x => FilterTypeSymbol(((IParameterSymbol)x).Type) }, 
				                 { SymbolKind.Method, x => FilterTypeSymbol(x.ContainingType) }, 
				                 { SymbolKind.Field, x => FilterTypeSymbol(((IFieldSymbol)x).Type) }, 
				                 { SymbolKind.Property, x => FilterTypeSymbol(x.ContainingType) }, 
				                 { SymbolKind.Event, x => FilterTypeSymbol(x.ContainingType) }
			                 };

			_classCouplingActions = new Dictionary<SyntaxKind, Action<SyntaxNode>>
			                        {
				                        { SyntaxKind.MethodDeclaration, x => CalculateMethodClassCoupling((MethodDeclarationSyntax)x) }, 
				                        { SyntaxKind.ConstructorDeclaration, x => CalculateConstructorCoupling((ConstructorDeclarationSyntax)x) }, 
				                        { SyntaxKind.DestructorDeclaration, x => CalculateConstructorCoupling((DestructorDeclarationSyntax)x) }, 
				                        { SyntaxKind.GetAccessorDeclaration, x => CalculateAccessorClassCoupling((AccessorDeclarationSyntax)x) }, 
				                        { SyntaxKind.SetAccessorDeclaration, x => CalculateAccessorClassCoupling((AccessorDeclarationSyntax)x) }, 
										{ SyntaxKind.EventFieldDeclaration, x => CalculateEventClassCoupling((EventFieldDeclarationSyntax)x) }, 
				                        { SyntaxKind.AddAccessorDeclaration, x => CalculateAccessorClassCoupling((AccessorDeclarationSyntax)x) }, 
				                        { SyntaxKind.RemoveAccessorDeclaration, x => CalculateAccessorClassCoupling((AccessorDeclarationSyntax)x) }
			                        };
		}

		public IEnumerable<ITypeCoupling> Calculate(SyntaxNode syntaxNode)
		{
			Action<SyntaxNode> action;

			if (_classCouplingActions.TryGetValue(syntaxNode.Kind(), out action))
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

		private void CalculateEventClassCoupling(EventFieldDeclarationSyntax syntax)
		{
			IdentifierNameSyntax node = (IdentifierNameSyntax)syntax.Declaration.Type;
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

		private void CalculateConstructorCoupling(BaseMethodDeclarationSyntax syntax)
		{
			Visit(syntax);
			if (syntax.Body != null)
			{
				CollectMemberCouplings(syntax.Body);
			}
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

		private void CalculateAccessorClassCoupling(AccessorDeclarationSyntax accessor)
		{
			var syntax = (BasePropertyDeclarationSyntax)accessor.Parent.Parent;
			FilterType(syntax.Type);

			var body = accessor.Body;
			if (body != null)
			{
				Visit(body);
				CollectMemberCouplings(body);
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
				.Where(x => x.Kind == SymbolKind.Method || x.Kind == SymbolKind.Property || x.Kind == SymbolKind.Event)
				.AsArray();
			_calledMethods.AddRange(methodCouplings.Where(x => x.Kind == SymbolKind.Method).Cast<IMethodSymbol>());
			_calledProperties.AddRange(methodCouplings.Where(x => x.Kind == SymbolKind.Property).Cast<IPropertySymbol>());
			_usedEvents.AddRange(methodCouplings.Where(x => x.Kind == SymbolKind.Event).Cast<IEventSymbol>());
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
