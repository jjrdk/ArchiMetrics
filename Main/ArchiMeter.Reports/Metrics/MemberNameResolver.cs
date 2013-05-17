namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using Core.Metrics;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class MemberNameResolver
	{
		// Fields
		private readonly ISemanticModel _semanticModel;

		// Methods
		public MemberNameResolver(ISemanticModel semanticModel)
		{
			this._semanticModel = semanticModel;
		}

		private void AppendMethodIdentifier(ConstructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(DestructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			ExplicitInterfaceSpecifierSyntax syntax2;
			IdentifierNameSyntax syntax3;
			if (((syntax2 = syntax.ExplicitInterfaceSpecifier) != null) && ((syntax3 = syntax2.Name as IdentifierNameSyntax) != null))
			{
				string valueText = syntax3.Identifier.ValueText;
				builder.AppendFormat("{0}.", valueText);
			}
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(PropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendParameters(BaseMethodDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			ParameterListSyntax parameterList = syntax.ParameterList;
			if (parameterList != null)
			{
				SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
				Func<ParameterSyntax, string> selector = x => string.Empty;
				if (parameters.Any())
				{
					selector = x =>
						{
							var b = new StringBuilder();
							var value = string.Join(" ", (from m in x.Modifiers select m.ValueText).ToArray<string>());
							if (!string.IsNullOrEmpty(value))
							{
								b.Append(value);
								b.Append(" ");
							}

							var symbol = _semanticModel.GetSymbolInfo(x.Type).Symbol as TypeSymbol;
							if (symbol == null)
							{
								return "?";
							}

							var str2 = ResolveTypeName(symbol);
							if (!string.IsNullOrWhiteSpace(str2))
							{
								b.Append(str2);
							}

							return b.ToString();
						};
				}

				string str = string.Join(", ", parameters.Select(selector).Where(x => !string.IsNullOrWhiteSpace(x)));
				builder.Append(str);
			}

			builder.Append(")");
		}

		private void AppendParameters(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			TypeSymbol symbol = _semanticModel.GetSymbolInfo(syntax.Type).Symbol as TypeSymbol;
			if (symbol != null)
			{
				string str = ResolveTypeName(symbol);
				builder.Append(str);
			}
			builder.Append(")");
		}

		private void AppendParameters(PropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			TypeSymbol symbol = _semanticModel.GetSymbolInfo(syntax.Type).Symbol as TypeSymbol;
			if (symbol != null)
			{
				string str = ResolveTypeName(symbol);
				builder.Append(str);
			}

			builder.Append(")");
		}

		private void AppendReturnType(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			TypeSymbol symbol = this._semanticModel.GetSymbolInfo(syntax.ReturnType, new CancellationToken()).Symbol as TypeSymbol;
			if (symbol != null)
			{
				string str = ResolveTypeName(symbol);
				builder.AppendFormat(" : {0}", str);
			}
		}

		private void AppendReturnType(PropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			TypeSymbol symbol = this._semanticModel.GetSymbolInfo(syntax.Type, new CancellationToken()).Symbol as TypeSymbol;
			if (symbol != null)
			{
				string str = ResolveTypeName(symbol);
				builder.AppendFormat(" : {0}", str);
			}
		}

		private static void AppendTypeParameters(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			if (syntax.TypeParameterList != null)
			{
				SeparatedSyntaxList<TypeParameterSyntax> parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					string str = string.Join(", ", (IEnumerable<string>)(from x in parameters select x.Identifier.ValueText));
					builder.AppendFormat("<{0}>", str);
				}
			}
		}

		private string GetAddEventHandlerSignatureString(EventDeclarationSyntax syntax)
		{
			StringBuilder builder = new StringBuilder();
			syntax.AccessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>(x => x.Kind == SyntaxKind.SetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, builder);
			builder.Append(".add");
			this.AppendParameters(syntax, builder);
			builder.Append(" : void");
			return builder.ToString();
		}

		public string GetConstructorSignatureString(ConstructorDeclarationSyntax syntax)
		{
			StringBuilder builder = new StringBuilder();
			this.AppendMethodIdentifier(syntax, builder);
			this.AppendParameters(syntax, builder);
			return builder.ToString();
		}

		private string GetDestructorSignatureString(DestructorDeclarationSyntax syntax)
		{
			StringBuilder builder = new StringBuilder();
			this.AppendMethodIdentifier(syntax, builder);
			return builder.ToString();
		}

		public string GetMethodSignatureString(MethodDeclarationSyntax syntax)
		{
			StringBuilder builder = new StringBuilder();
			this.AppendMethodIdentifier(syntax, builder);
			AppendTypeParameters(syntax, builder);
			this.AppendParameters(syntax, builder);
			this.AppendReturnType(syntax, builder);
			return builder.ToString();
		}

		public string GetPropertyGetterSignatureString(PropertyDeclarationSyntax syntax)
		{
			StringBuilder builder = new StringBuilder();
			syntax.AccessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>(x => x.Kind == SyntaxKind.GetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, builder);
			builder.Append(".get()");
			this.AppendReturnType(syntax, builder);
			return builder.ToString();
		}

		public string GetPropertySetterSignatureString(PropertyDeclarationSyntax syntax)
		{
			StringBuilder builder = new StringBuilder();
			syntax.AccessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>(x => x.Kind == SyntaxKind.SetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, builder);
			builder.Append(".set");
			this.AppendParameters(syntax, builder);
			builder.Append(" : void");
			return builder.ToString();
		}

		private string GetRemoveEventHandlerSignatureString(EventDeclarationSyntax syntax)
		{
			StringBuilder builder = new StringBuilder();
			syntax.AccessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>(x => x.Kind == SyntaxKind.SetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, builder);
			builder.Append(".remove");
			this.AppendParameters(syntax, builder);
			builder.Append(" : void");
			return builder.ToString();
		}

		private static string ResolveTypeName(TypeSymbol symbol)
		{
			NamedTypeSymbol symbol3;
			StringBuilder builder = new StringBuilder();
			bool flag = false;
			ArrayTypeSymbol symbol2 = symbol as ArrayTypeSymbol;
			if (symbol2 != null)
			{
				flag = true;
				symbol = symbol2.ElementType;
			}
			builder.Append(symbol.Name);
			if (((symbol3 = symbol as NamedTypeSymbol) != null) && symbol3.TypeArguments.Any())
			{
				IEnumerable<string> values = (from x in symbol3.TypeArguments.AsEnumerable() select ResolveTypeName(x)).ToArray<string>();
				builder.AppendFormat("<{0}>", string.Join(", ", values));
			}
			if (flag)
			{
				builder.Append("[]");
			}
			return builder.ToString();
		}

		public bool TryResolveMemberSignatureString(MemberNode member, out string signature)
		{
			Func<CommonSyntaxNode, string> func;
			signature = null;
			CommonSyntaxNode syntaxNode = member.SyntaxNode;
			Dictionary<MemberKind, Func<CommonSyntaxNode, string>> dictionary2 = new Dictionary<MemberKind, Func<CommonSyntaxNode, string>>();
			dictionary2.Add(MemberKind.Method, x => this.GetMethodSignatureString((MethodDeclarationSyntax)x));
			dictionary2.Add(MemberKind.Constructor, x => this.GetConstructorSignatureString((ConstructorDeclarationSyntax)x));
			dictionary2.Add(MemberKind.Destructor, x => this.GetDestructorSignatureString((DestructorDeclarationSyntax)x));
			dictionary2.Add(MemberKind.GetProperty, x => this.GetPropertyGetterSignatureString((PropertyDeclarationSyntax)x));
			dictionary2.Add(MemberKind.SetProperty, x => this.GetPropertySetterSignatureString((PropertyDeclarationSyntax)x));
			dictionary2.Add(MemberKind.AddEventHandler, x => this.GetAddEventHandlerSignatureString((EventDeclarationSyntax)x));
			dictionary2.Add(MemberKind.RemoveEventHandler, x => this.GetRemoveEventHandlerSignatureString((EventDeclarationSyntax)x));
			Dictionary<MemberKind, Func<CommonSyntaxNode, string>> dictionary = dictionary2;
			if (dictionary.TryGetValue(member.Kind, out func))
			{
				signature = func(syntaxNode);
				return !string.IsNullOrEmpty(signature);
			}
			return false;
		}
	}
}