// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberNameResolver.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberNameResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

	internal sealed class MemberNameResolver
	{
		private readonly SemanticModel _semanticModel;

		public MemberNameResolver(SemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		public string TryResolveMemberSignatureString(SyntaxNode syntaxNode)
		{
			Func<SyntaxNode, string> func;
			var dictionary2 = new Dictionary<SyntaxKind, Func<SyntaxNode, string>>
				                  {
					                  { SyntaxKind.MethodDeclaration, x => GetMethodSignatureString((MethodDeclarationSyntax)x) }, 
					                  { SyntaxKind.ConstructorDeclaration, x => GetConstructorSignatureString((ConstructorDeclarationSyntax)x) }, 
					                  { SyntaxKind.DestructorDeclaration, x => GetDestructorSignatureString((DestructorDeclarationSyntax)x) }, 
					                  { SyntaxKind.GetAccessorDeclaration, x => GetPropertyGetterSignatureString((PropertyDeclarationSyntax)x) }, 
					                  { SyntaxKind.SetAccessorDeclaration, x => GetPropertySetterSignatureString((PropertyDeclarationSyntax)x) }, 
					                  { SyntaxKind.AddAccessorDeclaration, x => GetAddEventHandlerSignatureString((EventDeclarationSyntax)x) }, 
					                  { SyntaxKind.RemoveAccessorDeclaration, x => GetRemoveEventHandlerSignatureString((EventDeclarationSyntax)x) }
				                  };
			var dictionary = dictionary2;
			return dictionary.TryGetValue(syntaxNode.CSharpKind(), out func)
				? func(syntaxNode)
				: string.Empty;
		}

		private static string ResolveTypeName(ITypeSymbol symbol)
		{
			INamedTypeSymbol symbol3;
			var builder = new StringBuilder();
			var flag = false;
			var symbol2 = symbol as IArrayTypeSymbol;
			if (symbol2 != null)
			{
				flag = true;
				symbol = symbol2.ElementType;
			}

			builder.Append(symbol.Name);
			if (((symbol3 = symbol as INamedTypeSymbol) != null) && symbol3.TypeArguments.Any())
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

		private static void AppendMethodIdentifier(ConstructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private static void AppendMethodIdentifier(DestructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private static void AppendMethodIdentifier(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private static void AppendMethodIdentifier(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			ExplicitInterfaceSpecifierSyntax syntax2;
			IdentifierNameSyntax syntax3;
			if (((syntax2 = syntax.ExplicitInterfaceSpecifier) != null) && ((syntax3 = syntax2.Name as IdentifierNameSyntax) != null))
			{
				var valueText = syntax3.Identifier.ValueText;
				builder.AppendFormat("{0}.", valueText);
			}

			builder.Append(syntax.Identifier.ValueText);
		}

		private static void AppendMethodIdentifier(PropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private static void AppendTypeParameters(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			if (syntax.TypeParameterList != null)
			{
				var parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					var parameterNames = string.Join(", ", from x in parameters select x.Identifier.ValueText);
					builder.AppendFormat("<{0}>", parameterNames);
				}
			}
		}

		private static string GetDestructorSignatureString(DestructorDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax, builder);
			return builder.ToString();
		}

		private string GetConstructorSignatureString(ConstructorDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax, builder);
			AppendParameters(syntax, builder);
			return builder.ToString();
		}

		private string GetMethodSignatureString(MethodDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax, builder);
			AppendTypeParameters(syntax, builder);
			AppendParameters(syntax, builder);
			AppendReturnType(syntax, builder);
			return builder.ToString();
		}

		private string GetPropertyGetterSignatureString(PropertyDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax, builder);
			builder.Append(".get()");
			AppendReturnType(syntax, builder);
			return builder.ToString();
		}

		private string GetPropertySetterSignatureString(PropertyDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax, builder);
			builder.Append(".set");
			AppendParameters(syntax, builder);
			builder.Append(" : void");
			return builder.ToString();
		}

		private string GetAddEventHandlerSignatureString(EventDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax, builder);
			builder.Append(".add");
			AppendParameters(syntax, builder);
			builder.Append(" : void");
			return builder.ToString();
		}

		private void AppendParameters(BaseMethodDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			var parameterList = syntax.ParameterList;
			if (parameterList != null)
			{
				var parameters = parameterList.Parameters;
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

							var symbol = _semanticModel.GetSymbolInfo(x.Type).Symbol as ITypeSymbol;
							if (symbol == null)
							{
								return "?";
							}

							var typeName = ResolveTypeName(symbol);
							if (!string.IsNullOrWhiteSpace(typeName))
							{
								b.Append(typeName);
							}

							return b.ToString();
						};
				}

				var parameterNames = string.Join(", ", parameters.Select(selector).Where(x => !string.IsNullOrWhiteSpace(x)));
				builder.Append(parameterNames);
			}

			builder.Append(")");
		}

		private void AppendParameters(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			var symbol = _semanticModel.GetSymbolInfo(syntax.Type).Symbol as ITypeSymbol;
			if (symbol != null)
			{
				var typeName = ResolveTypeName(symbol);
				builder.Append(typeName);
			}

			builder.Append(")");
		}

		private void AppendParameters(BasePropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			var symbol = _semanticModel.GetSymbolInfo(syntax.Type).Symbol as ITypeSymbol;
			if (symbol != null)
			{
				var typeName = ResolveTypeName(symbol);
				builder.Append(typeName);
			}

			builder.Append(")");
		}

		private void AppendReturnType(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			var symbolInfo = _semanticModel.GetSymbolInfo(syntax.ReturnType);
			var symbol = symbolInfo.Symbol as ITypeSymbol;
			if (symbol != null)
			{
				var typeName = ResolveTypeName(symbol);
				builder.AppendFormat(" : {0}", typeName);
			}
		}

		private void AppendReturnType(BasePropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			var symbol = _semanticModel.GetSymbolInfo(syntax.Type).Symbol as ITypeSymbol;
			if (symbol != null)
			{
				var typeName = ResolveTypeName(symbol);
				builder.AppendFormat(" : {0}", typeName);
			}
		}

		private string GetRemoveEventHandlerSignatureString(EventDeclarationSyntax syntax)
		{
			var builder = new StringBuilder();
			AppendMethodIdentifier(syntax, builder);
			builder.Append(".remove");
			AppendParameters(syntax, builder);
			builder.Append(" : void");
			return builder.ToString();
		}
	}
}
