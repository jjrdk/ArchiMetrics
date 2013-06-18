// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepthOfInheritanceAnalyzer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DepthOfInheritanceAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	internal sealed class DepthOfInheritanceAnalyzer
	{
		// Fields
		private readonly IEnumerable<TypeKind> _inheritableTypes = new[] { TypeKind.Class, TypeKind.Struct };
		private readonly ISemanticModel _semanticModel;

		// Methods
		public DepthOfInheritanceAnalyzer(ISemanticModel semanticModel)
		{
			//////Verify.NotNull<ISemanticModel>(Expression.Lambda<Func<ISemanticModel>>(Expression.Constant(semanticModel), new ParameterExpression[0]), (string)null);
			_semanticModel = semanticModel;
		}

		public int Calculate(TypeDeclarationSyntax type)
		{
			int num = type.Kind == SyntaxKind.ClassDeclaration || type.Kind == SyntaxKind.StructDeclaration ? 1 : 0;
			if (type.BaseList != null)
			{
				foreach (var syntax in type.BaseList.Types)
				{
					CommonSymbolInfo symbolInfo = _semanticModel.GetSymbolInfo(syntax);
					for (var symbol = symbolInfo.Symbol as NamedTypeSymbol; symbol != null; symbol = symbol.BaseType)
					{
						if (_inheritableTypes.Any(x => x == symbol.TypeKind))
						{
							num++;
						}
					}
				}
			}

			return num == 0 && (type.Kind == SyntaxKind.ClassDeclaration || type.Kind == SyntaxKind.StructDeclaration)
				? 1
				: num;
		}
	}
}