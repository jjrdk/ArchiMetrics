// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberBodySelector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberBodySelector type.
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

	internal sealed class MemberBodySelector
	{
		// Methods
		public static BlockSyntax FindBody(MemberNode node)
		{
			Func<CommonSyntaxNode, BlockSyntax> func;
			BlockSyntax syntax;
			var dictionary2 = new Dictionary<MemberKind, Func<CommonSyntaxNode, BlockSyntax>>
				                  {
					                  { MemberKind.Method, x => ((MethodDeclarationSyntax)x).Body }, 
					                  { MemberKind.Constructor, x => ((ConstructorDeclarationSyntax)x).Body }, 
					                  { MemberKind.Destructor, x => ((DestructorDeclarationSyntax)x).Body }, 
					                  { MemberKind.GetProperty, x => GetPropertyAccessorBody((PropertyDeclarationSyntax)x, SyntaxKind.GetAccessorDeclaration) }, 
					                  { MemberKind.SetProperty, x => GetPropertyAccessorBody((PropertyDeclarationSyntax)x, SyntaxKind.SetAccessorDeclaration) }, 
					                  { MemberKind.AddEventHandler, x => GetEventAccessorBody((EventDeclarationSyntax)x, SyntaxKind.AddAccessorDeclaration) }, 
					                  { MemberKind.RemoveEventHandler, x => GetEventAccessorBody((EventDeclarationSyntax)x, SyntaxKind.RemoveAccessorDeclaration) }
				                  };
			Dictionary<MemberKind, Func<CommonSyntaxNode, BlockSyntax>> dictionary = dictionary2;
			if (dictionary.TryGetValue(node.Kind, out func) && ((syntax = func(node.SyntaxNode)) != null))
			{
				return syntax;
			}

			return null;
		}

		private static BlockSyntax GetEventAccessorBody(EventDeclarationSyntax syntax, SyntaxKind kind)
		{
			var syntax2 = syntax.AccessorList.Accessors.SingleOrDefault(a => a.Kind == kind);
			return syntax2 != null ? syntax2.Body : null;
		}

		private static BlockSyntax GetPropertyAccessorBody(PropertyDeclarationSyntax syntax, SyntaxKind kind)
		{
			var syntax2 = syntax.AccessorList.Accessors.SingleOrDefault(a => a.Kind == kind);
			return syntax2 != null ? syntax2.Body : null;
		}
	}
}
