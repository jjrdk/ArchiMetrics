namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Core.Metrics;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class MemberClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		private readonly Dictionary<CommonSymbolKind, Action<ISymbol>> _symbolActions;
		private readonly Dictionary<MemberKind, Action<SyntaxNode>> _classCouplingActions;

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
					                        { MemberKind.Method, x => this.CalculateMethodClassCoupling((MethodDeclarationSyntax)x) },
					                        { MemberKind.Constructor, x => this.CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x) },
					                        { MemberKind.Destructor, x => this.CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x) },
					                        { MemberKind.GetProperty, x => this.CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.GetAccessorDeclaration) },
					                        { MemberKind.SetProperty, x => this.CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.SetAccessorDeclaration) },
					                        { MemberKind.AddEventHandler, x => this.CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.AddAccessorDeclaration) },
					                        { MemberKind.RemoveEventHandler, x => this.CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.RemoveAccessorDeclaration) }
				                        };
		}

		public IEnumerable<string> Calculate(MemberNode memberNode)
		{
			Action<SyntaxNode> action;
			var syntaxNode = (SyntaxNode)memberNode.SyntaxNode;
			if (_classCouplingActions.TryGetValue(memberNode.Kind, out action))
			{
				action(syntaxNode);
			}
			return GetCollectedTypesNames();
		}

		private void CalculateEventClassCoupling(EventDeclarationSyntax syntax, SyntaxKind kind)
		{
			FilterType(syntax.Type);
			BlockSyntax accessor = GetAccessor(syntax.AccessorList, kind);
			if (accessor != null)
			{
				this.Visit(accessor);
			}
		}

		private void CalculateGenericMemberClassCoupling(MemberDeclarationSyntax syntax)
		{
			this.Visit(syntax);
		}

		private void CalculateMethodClassCoupling(MethodDeclarationSyntax syntax)
		{
			this.Visit(syntax);
			FilterType(syntax.ReturnType);
		}

		private void CalculatePropertyClassCoupling(PropertyDeclarationSyntax syntax, SyntaxKind kind)
		{
			FilterType(syntax.Type);
			BlockSyntax accessor = GetAccessor(syntax.AccessorList, kind);
			if (accessor != null)
			{
				this.Visit(accessor);
			}
		}

		private static BlockSyntax GetAccessor(AccessorListSyntax accessorList, SyntaxKind kind)
		{
			return accessorList.Accessors.Single(x => x.Kind == kind).Body;
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
	}
}