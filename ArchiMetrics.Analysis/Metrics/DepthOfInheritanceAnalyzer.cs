// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepthOfInheritanceAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DepthOfInheritanceAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class DepthOfInheritanceAnalyzer
	{
		private readonly IEnumerable<TypeKind> _inheritableTypes = new[] { TypeKind.Class, TypeKind.Struct };
		private readonly SemanticModel _semanticModel;

		public DepthOfInheritanceAnalyzer(SemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		public int Calculate(TypeDeclarationSyntax type)
		{
			var num = type.IsKind(SyntaxKind.ClassDeclaration) || type.IsKind(SyntaxKind.StructDeclaration) ? 1 : 0;
			if (type.BaseList != null)
			{
				foreach (var symbolInfo in type.BaseList.Types.Select(syntax => ModelExtensions.GetSymbolInfo(_semanticModel, syntax)))
				{
					for (var symbol = symbolInfo.Symbol as INamedTypeSymbol; symbol != null; symbol = symbol.BaseType)
					{
						if (_inheritableTypes.Any(x => x == symbol.TypeKind))
						{
							num++;
						}
					}
				}
			}

			return num == 0 && (type.IsKind(SyntaxKind.ClassDeclaration) || type.IsKind(SyntaxKind.StructDeclaration))
				? 1
				: num;
		}
	}
}
