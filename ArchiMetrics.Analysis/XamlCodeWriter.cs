// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlCodeWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XamlCodeWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMetrics.Common.Xaml;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Formatting;

	internal class XamlCodeWriter
	{
		private static readonly Regex BindingRegex = new Regex("^{Binding(.+)?}$", RegexOptions.Compiled);

		public SyntaxTree CreateSyntax(XamlNode node)
		{
			var properties = node.Properties.SelectMany(CreatePropertySyntax);
			var assignments = node.Properties.SelectMany(p => CreatePropertyAssignment(p, SyntaxFactory.ThisExpression()));

			var codeRoot = properties.Concat(assignments).Concat(CreateSyntax(node.Children));
			var classDeclarationSyntax =
				!string.IsNullOrWhiteSpace(node.BaseClassName)
					? SyntaxFactory.ClassDeclaration(node.ClassName)
							.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
							.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
					: SyntaxFactory.ClassDeclaration(node.ClassName)
							.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
							.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
							.WithBaseList(SyntaxFactory.BaseList(
								SyntaxFactory.SeparatedList(
									new[] { SyntaxFactory.ParseTypeName(node.BaseClassName) })));

			var xamlClass = classDeclarationSyntax
				.WithMembers(
					SyntaxFactory.List<MemberDeclarationSyntax>(
						new[]
						{
							SyntaxFactory.MethodDeclaration(
								SyntaxFactory.PredefinedType(
									SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
								"InitializeComponent")
								.WithModifiers(
									SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)))
								.WithBody(SyntaxFactory.Block(codeRoot))
						}));
			var usings = SyntaxFactory.List(node.Usings.Select(kvp => string.IsNullOrWhiteSpace(kvp.Value)
																   ? SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(kvp.Key))
																   : SyntaxFactory.UsingDirective(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(kvp.Value)), SyntaxFactory.ParseName(kvp.Key))));
			var namespaceName = string.IsNullOrWhiteSpace(node.NamespaceName) ? "ArchiMetrics" : node.NamespaceName;
			var namespaceSyntax =
				SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
					.WithUsings(usings)
					.AddMembers(xamlClass);
			//.Format(FormattingOptions.SmartIndent)
			//.GetFormattedRoot() as MemberDeclarationSyntax;
			var compilationUnit = SyntaxFactory.CompilationUnit().AddMembers(namespaceSyntax);
			return CSharpSyntaxTree.Create(compilationUnit);
		}

		private static ExpressionStatementSyntax CreateDependencyPropertySyntax(string variableName, XamlNodeAttribute attr)
		{
			var memberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(variableName), SyntaxFactory.IdentifierName("SetValue"));
			var argList = SyntaxFactory.SeparatedList(
				new[] { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(attr.VariableName + "Property")), SyntaxFactory.Argument(SyntaxFactory.IdentifierName(attr.Value)) },
				new[] { SyntaxFactory.Token(SyntaxKind.CommaToken) });
			var invocation = SyntaxFactory.InvocationExpression(memberAccess, SyntaxFactory.ArgumentList(argList));
			return SyntaxFactory.ExpressionStatement(invocation);
		}

		private static LocalDeclarationStatementSyntax CreateLocalDeclaration(XamlNode childNode)
		{
			var variables = SyntaxFactory.SeparatedList(
				new[]
				{
					SyntaxFactory.VariableDeclarator(
						SyntaxFactory.Identifier(childNode.VariableName),
						null,
						SyntaxFactory.EqualsValueClause(
							SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(childNode.ClassName), SyntaxFactory.ArgumentList(), null)))
				});
			return SyntaxFactory.LocalDeclarationStatement(
				SyntaxFactory.VariableDeclaration(
					SyntaxFactory.IdentifierName("var"),
					variables));
		}

		private static ExpressionStatementSyntax CreateParentSyntax(XamlNode childNode)
		{
			var parent = childNode.Parent;
			switch (parent.ClassName)
			{
				case "Canvas":
				case "StackPanel":
				case "DockPanel":
				case "Grid":
					{
						var childrenAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(parent.VariableName), SyntaxFactory.IdentifierName("Children"));
						var addAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, childrenAccess, SyntaxFactory.IdentifierName("Add"));
						return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(addAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[] { SyntaxFactory.Argument(SyntaxFactory.IdentifierName(childNode.VariableName)) }))));
					}

				case "Border":
					{
						var childAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(parent.VariableName), SyntaxFactory.IdentifierName("Child"));
						return SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(
							SyntaxKind.AssignExpression,
							childAccess,
							SyntaxFactory.Token(SyntaxKind.EqualsToken),
							SyntaxFactory.IdentifierName(childNode.VariableName)));
					}

				case "Window":
				case "UserControl":
					{
						var contentAccess = SyntaxFactory.MemberAccessExpression(
							SyntaxKind.MemberAccessExpression,
							SyntaxFactory.IdentifierName(parent.VariableName),
							SyntaxFactory.IdentifierName("Content"));
						return SyntaxFactory.ExpressionStatement(
							SyntaxFactory.BinaryExpression(
								SyntaxKind.AssignExpression,
								contentAccess,
								SyntaxFactory.Token(SyntaxKind.EqualsToken),
								SyntaxFactory.IdentifierName(childNode.VariableName)));
					}

				case "ResourceDictionary":
					{
						var
							addAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(parent.VariableName), SyntaxFactory.IdentifierName("Add"));
						var keyAttribute = childNode.Attributes.First(a => a.VariableName == "Key");
						var arguments = new[]
							                {
								                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(keyAttribute.Value)), 
												SyntaxFactory.Argument(SyntaxFactory.IdentifierName(childNode.VariableName))
							                };
						var separators = new[] { SyntaxFactory.Token(SyntaxKind.CommaToken) };
						return SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(addAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments, separators))));
					}

				default:
					var variableAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(parent.VariableName), SyntaxFactory.IdentifierName("AddChild"));
					return
						SyntaxFactory.ExpressionStatement(
							SyntaxFactory.InvocationExpression(
								variableAccess,
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SeparatedList(
										new[]
										{
											SyntaxFactory.Argument(SyntaxFactory.IdentifierName(childNode.VariableName))
										}))));
			}
		}

		private static bool IsPropertyOf(XamlNode parent, XamlNode child)
		{
			var p = parent.Properties;
			return p.Any(n => n.Name == child.VariableName);
		}

		private static StatementSyntax CreateBindingSyntax(
			string nodeClassName,
			string nodeVariableName,
			string variableName,
			string inlineValue)
		{
			var strings = inlineValue.Split(',').ToArray();
			var parameters = strings.SelectMany(p => p.Trim().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
									.Select(kv => kv.Split('='))
									.Select(t => new Tuple<string, string>(t.First(), string.Join("=", t.Skip(1))))
									.Select(t => SyntaxFactory.BinaryExpression(SyntaxKind.AssignExpression, SyntaxFactory.IdentifierName(t.Item1), SyntaxFactory.IdentifierName(t.Item2.Replace("'", "\""))));

			var separators = Enumerable.Repeat(SyntaxFactory.Token(SyntaxKind.CommaToken), strings.Length - 1);
			var binding = SyntaxFactory.ObjectCreationExpression(
				SyntaxFactory.ParseTypeName("Binding"),
				null,
				SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>(parameters, separators)));
			var memberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(nodeVariableName), SyntaxFactory.IdentifierName("SetBinding"));
			var dependencyProperty =
				SyntaxFactory.Argument(SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(nodeClassName), SyntaxFactory.IdentifierName(variableName + "Property")));
			var arguments = new[] { dependencyProperty, SyntaxFactory.Argument(binding) };
			var memberInvocation = SyntaxFactory.InvocationExpression(memberAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments, new[] { SyntaxFactory.Token(SyntaxKind.CommaToken) })));
			return SyntaxFactory.ExpressionStatement(memberInvocation);
		}

		private ExpressionSyntax GetPropertyIdentifier(XamlPropertyNode prop)
		{
			if (prop.Value == null)
			{
				return SyntaxFactory.IdentifierName(SyntaxFactory.Token(SyntaxKind.NullKeyword));
			}

			var node = prop.Value as XamlNode;
			return SyntaxFactory.IdentifierName(node != null
											 ? node.VariableName
											 : prop.Value.ToString());
		}

		private IEnumerable<StatementSyntax> WriteProperties(Func<XamlPropertyNode, IEnumerable<StatementSyntax>> propertyCreation, XamlNode childNode)
		{
			foreach (var prop in childNode.Properties)
			{
				foreach (var created in propertyCreation(prop))
				{
					yield return created;
				}

				var memberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(childNode.VariableName), SyntaxFactory.IdentifierName(prop.Name));
				var setAccess = SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.AssignExpression, memberAccess, SyntaxFactory.Token(SyntaxKind.EqualsToken), GetPropertyIdentifier(prop)));

				yield return setAccess;
			}
		}

		private IEnumerable<StatementSyntax> WriteAttributes(XamlNode childNode)
		{
			foreach (var attr in childNode.Attributes)
			{
				if (!string.IsNullOrWhiteSpace(attr.ExtraCode))
				{
					yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.ParseExpression(attr.ExtraCode));
				}

				var bindingMatch = BindingRegex.Match(attr.Value);
				if (bindingMatch.Success)
				{
					var inlineValue = bindingMatch.Groups[1].Value.Trim();
					yield return CreateBindingSyntax(childNode.ClassName, childNode.VariableName, attr.VariableName, inlineValue);
				}
				else if (attr.VariableName.Contains("."))
				{
					yield return CreateDependencyPropertySyntax(childNode.VariableName, attr);
				}
				else
				{
					var memberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, SyntaxFactory.IdentifierName(childNode.VariableName), SyntaxFactory.IdentifierName(attr.VariableName));
					yield return SyntaxFactory.ExpressionStatement(
						SyntaxFactory.BinaryExpression(
							SyntaxKind.AssignExpression, memberAccess, SyntaxFactory.Token(SyntaxKind.EqualsToken), SyntaxFactory.IdentifierName(attr.Value)));
				}
			}
		}

		private IEnumerable<StatementSyntax> GenerateChildStatement(XamlNode childNode, Func<XamlPropertyNode, IEnumerable<StatementSyntax>> propertyCreation)
		{
			yield return CreateLocalDeclaration(childNode);
			foreach (var property in WriteProperties(propertyCreation, childNode))
			{
				yield return property;
			}

			foreach (var attribute in WriteAttributes(childNode))
			{
				yield return attribute;
			}

			if (childNode.Parent != null && !IsPropertyOf(childNode.Parent, childNode))
			{
				yield return CreateParentSyntax(childNode);
				foreach (var child in childNode.Children.SelectMany(c => GenerateChildStatement(c, propertyCreation)))
				{
					yield return child;
				}
			}
		}

		private IEnumerable<ExpressionStatementSyntax> CreatePropertyAssignment(XamlPropertyNode prop, ExpressionSyntax ownerExpression)
		{
			var values = prop.Value as IEnumerable<XamlNode>;
			if (values != null)
			{
				var statements = values.Select(v =>
					{
						var memberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, ownerExpression, SyntaxFactory.IdentifierName(prop.Name));
						var addAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, memberAccess, SyntaxFactory.IdentifierName("Add"));
						return
							SyntaxFactory.ExpressionStatement(
								SyntaxFactory.InvocationExpression(
									addAccess,
									SyntaxFactory.ArgumentList(new[]
															   {
																   SyntaxFactory.SeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(v.VariableName)))
															   })));
					});

				foreach (var statement in statements)
				{
					yield return statement;
				}
			}
			else
			{
				var memberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.MemberAccessExpression, ownerExpression, SyntaxFactory.IdentifierName(prop.Name));
				yield return SyntaxFactory.ExpressionStatement(SyntaxFactory.BinaryExpression(SyntaxKind.AssignExpression, memberAccess, SyntaxFactory.Token(SyntaxKind.EqualsToken), GetPropertyIdentifier(prop)));
			}
		}

		private IEnumerable<StatementSyntax> CreatePropertySyntax(XamlPropertyNode property)
		{
			var node = property.Value as XamlNode;
			if (node != null)
			{
				return GenerateChildStatement(node, CreatePropertySyntax);
			}

			var nodes = property.Value as IEnumerable<XamlNode>;
			if (nodes != null)
			{
				return nodes.SelectMany(n => GenerateChildStatement(n, CreatePropertySyntax));
			}

			return Enumerable.Empty<StatementSyntax>();
		}

		private IEnumerable<StatementSyntax> CreateSyntax(IEnumerable<XamlNode> nodes)
		{
			return nodes.SelectMany(n => GenerateChildStatement(n, CreatePropertySyntax))
					 .Select(s => s.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed));
		}
	}
}
