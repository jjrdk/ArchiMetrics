// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlCodeWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
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
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;
	using Roslyn.Services.Formatting;

	internal class XamlCodeWriter
	{
		private static readonly Regex BindingRegex = new Regex("^{Binding(.+)?}$", RegexOptions.Compiled);

		public SyntaxTree CreateSyntax(XamlNode node)
		{
			var properties = node.Properties.SelectMany(p => CreatePropertySyntax(p, Syntax.ThisExpression()));
			var assignments = node.Properties.SelectMany(p => CreatePropertyAssignment(p, Syntax.ThisExpression()));

			var codeRoot = properties.Concat(assignments).Concat(CreateSyntax(node.Children, Syntax.IdentifierName(node.VariableName)));
			var classDeclarationSyntax =
				!string.IsNullOrWhiteSpace(node.BaseClassName)
					? Syntax.ClassDeclaration(node.ClassName)
							.AddModifiers(Syntax.Token(SyntaxKind.PublicKeyword))
							.AddModifiers(Syntax.Token(SyntaxKind.PartialKeyword))
					: Syntax.ClassDeclaration(node.ClassName)
							.AddModifiers(Syntax.Token(SyntaxKind.PublicKeyword))
							.AddModifiers(Syntax.Token(SyntaxKind.PartialKeyword))
							.WithBaseList(Syntax.BaseList(
								Syntax.SeparatedList(
									Syntax.ParseTypeName(node.BaseClassName))));

			var xamlClass = classDeclarationSyntax
				.WithMembers(
					Syntax.List<MemberDeclarationSyntax>(
						Syntax.MethodDeclaration(
							Syntax.PredefinedType(
								Syntax.Token(SyntaxKind.VoidKeyword)), 
							"InitializeComponent")
							  .WithModifiers(
								  Syntax.TokenList(Syntax.Token(SyntaxKind.ProtectedKeyword)))
							  .WithBody(Syntax.Block(codeRoot))));
			var usings = Syntax.List(node.Usings.Select(kvp => string.IsNullOrWhiteSpace(kvp.Value)
																   ? Syntax.UsingDirective(Syntax.ParseName(kvp.Key))
																   : Syntax.UsingDirective(Syntax.NameEquals(Syntax.IdentifierName(kvp.Value)), Syntax.ParseName(kvp.Key))));
			var namespaceName = string.IsNullOrWhiteSpace(node.NamespaceName) ? "ArchiMetrics" : node.NamespaceName;
			var namespaceSyntax =
				Syntax.NamespaceDeclaration(Syntax.ParseName(namespaceName))
				.WithUsings(usings)
				.AddMembers(xamlClass)
				.Format(FormattingOptions.GetDefaultOptions())
				.GetFormattedRoot() as MemberDeclarationSyntax;
			var compilationUnit = Syntax.CompilationUnit().AddMembers(namespaceSyntax);
			return SyntaxTree.Create(compilationUnit);
		}

		private static ExpressionStatementSyntax CreateDependencyPropertySyntax(string variableName, XamlNodeAttribute attr)
		{
			var memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(variableName), Syntax.IdentifierName("SetValue"));
			var argList = Syntax.SeparatedList(
				new[] { Syntax.Argument(Syntax.IdentifierName(attr.VariableName + "Property")), Syntax.Argument(Syntax.IdentifierName(attr.Value)) }, 
				new[] { Syntax.Token(SyntaxKind.CommaToken) });
			var invocation = Syntax.InvocationExpression(memberAccess, Syntax.ArgumentList(argList));
			return Syntax.ExpressionStatement(invocation);
		}

		private static LocalDeclarationStatementSyntax CreateLocalDeclaration(XamlNode childNode)
		{
			var variables = Syntax.SeparatedList(
				Syntax.VariableDeclarator(
					Syntax.Identifier(childNode.VariableName), 
					null, 
					Syntax.EqualsValueClause(Syntax.ObjectCreationExpression(Syntax.ParseTypeName(childNode.ClassName), Syntax.ArgumentList(), null))));
			return Syntax.LocalDeclarationStatement(
				Syntax.VariableDeclaration(
					Syntax.IdentifierName("var"), 
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
						var childrenAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("Children"));
						var addAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, childrenAccess, Syntax.IdentifierName("Add"));
						return Syntax.ExpressionStatement(Syntax.InvocationExpression(addAccess, Syntax.ArgumentList(Syntax.SeparatedList(Syntax.Argument(Syntax.IdentifierName(childNode.VariableName))))));
					}

				case "Border":
					{
						var childAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("Child"));
						return Syntax.ExpressionStatement(Syntax.BinaryExpression(
							SyntaxKind.AssignExpression, 
							childAccess, 
							Syntax.Token(SyntaxKind.EqualsToken), 
							Syntax.IdentifierName(childNode.VariableName)));
					}

				case "Window":
				case "UserControl":
				{
					var contentAccess = Syntax.MemberAccessExpression(
						SyntaxKind.MemberAccessExpression, 
						Syntax.IdentifierName(parent.VariableName), 
						Syntax.IdentifierName("Content"));
						return Syntax.ExpressionStatement(
							Syntax.BinaryExpression(
								SyntaxKind.AssignExpression, 
								contentAccess, 
								Syntax.Token(SyntaxKind.EqualsToken), 
								Syntax.IdentifierName(childNode.VariableName)));
					}

				case "ResourceDictionary":
					{
						var
							addAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("Add"));
						var keyAttribute = childNode.Attributes.First(a => a.VariableName == "Key");
						var arguments = new[]
							                {
								                Syntax.Argument(Syntax.IdentifierName(keyAttribute.Value)), 
												Syntax.Argument(Syntax.IdentifierName(childNode.VariableName))
							                };
						var separators = new[] { Syntax.Token(SyntaxKind.CommaToken) };
						return Syntax.ExpressionStatement(Syntax.InvocationExpression(addAccess, Syntax.ArgumentList(Syntax.SeparatedList(arguments, separators))));
					}

				default:
					var variableAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("AddChild"));
					return
						Syntax.ExpressionStatement(
							Syntax.InvocationExpression(
								variableAccess, 
								Syntax.ArgumentList(Syntax.SeparatedList(Syntax.Argument(Syntax.IdentifierName(childNode.VariableName))))));
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
									.Select(t => Syntax.BinaryExpression(SyntaxKind.AssignExpression, Syntax.IdentifierName(t.Item1), Syntax.IdentifierName(t.Item2.Replace("'", "\""))));

			var separators = Enumerable.Repeat(Syntax.Token(SyntaxKind.CommaToken), strings.Length - 1);
			var binding = Syntax.ObjectCreationExpression(
				Syntax.ParseTypeName("Binding"), 
				null, 
				Syntax.InitializerExpression(SyntaxKind.ObjectInitializerExpression, Syntax.SeparatedList<ExpressionSyntax>(parameters, separators)));
			var memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(nodeVariableName), Syntax.IdentifierName("SetBinding"));
			var dependencyProperty =
				Syntax.Argument(Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(nodeClassName), Syntax.IdentifierName(variableName + "Property")));
			var arguments = new[] { dependencyProperty, Syntax.Argument(binding) };
			var memberInvocation = Syntax.InvocationExpression(memberAccess, Syntax.ArgumentList(Syntax.SeparatedList(arguments, new[] { Syntax.Token(SyntaxKind.CommaToken) })));
			return Syntax.ExpressionStatement(memberInvocation);
		}

		private ExpressionSyntax GetPropertyIdentifier(XamlPropertyNode prop)
		{
			if (prop.Value == null)
			{
				return Syntax.IdentifierName(Syntax.Token(SyntaxKind.NullKeyword));
			}

			var node = prop.Value as XamlNode;
			return Syntax.IdentifierName(node != null
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

				var memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(childNode.VariableName), Syntax.IdentifierName(prop.Name));
				var setAccess = Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, memberAccess, Syntax.Token(SyntaxKind.EqualsToken), GetPropertyIdentifier(prop)));

				yield return setAccess;
			}
		}

		private IEnumerable<StatementSyntax> WriteAttributes(XamlNode childNode)
		{
			foreach (var attr in childNode.Attributes)
			{
				if (!string.IsNullOrWhiteSpace(attr.ExtraCode))
				{
					yield return Syntax.ExpressionStatement(Syntax.ParseExpression(attr.ExtraCode));
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
					var memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(childNode.VariableName), Syntax.IdentifierName(attr.VariableName));
					yield return Syntax.ExpressionStatement(
						Syntax.BinaryExpression(
							SyntaxKind.AssignExpression, memberAccess, Syntax.Token(SyntaxKind.EqualsToken), Syntax.IdentifierName(attr.Value)));
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
						var memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, ownerExpression, Syntax.IdentifierName(prop.Name));
						var addAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, memberAccess, Syntax.IdentifierName("Add"));
						return Syntax.ExpressionStatement(Syntax.InvocationExpression(addAccess, Syntax.ArgumentList(Syntax.SeparatedList(Syntax.Argument(Syntax.IdentifierName(v.VariableName))))));
					});

				foreach (var statement in statements)
				{
					yield return statement;
				}
			}
			else
			{
				var memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, ownerExpression, Syntax.IdentifierName(prop.Name));
				yield return Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, memberAccess, Syntax.Token(SyntaxKind.EqualsToken), GetPropertyIdentifier(prop)));
			}
		}

		private IEnumerable<StatementSyntax> CreatePropertySyntax(XamlPropertyNode property, ExpressionSyntax ownerSyntax)
		{
			var node = property.Value as XamlNode;
			if (node != null)
			{
				return GenerateChildStatement(node, p => CreatePropertySyntax(p, ownerSyntax));
			}

			var nodes = property.Value as IEnumerable<XamlNode>;
			if (nodes != null)
			{
				return nodes.SelectMany(n => GenerateChildStatement(n, p => CreatePropertySyntax(p, ownerSyntax)));
			}

			return Enumerable.Empty<StatementSyntax>();
		}

		private IEnumerable<StatementSyntax> CreateSyntax(IEnumerable<XamlNode> nodes, ExpressionSyntax ownerSyntax)
		{
			return nodes.SelectMany(n => GenerateChildStatement(n, p => CreatePropertySyntax(p, ownerSyntax)))
					 .Select(s => s.WithTrailingTrivia(Syntax.ElasticCarriageReturnLineFeed));
		}
	}
}
