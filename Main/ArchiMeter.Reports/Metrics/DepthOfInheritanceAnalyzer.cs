namespace ArchiMeter.Reports.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;

	internal sealed class DepthOfInheritanceAnalyzer
	{

		private readonly IEnumerable<TypeKind> _inheritableTypes = new[] { TypeKind.Class, TypeKind.Struct };
		private readonly ISemanticModel _semanticModel;


		public DepthOfInheritanceAnalyzer(ISemanticModel semanticModel)
		{

			_semanticModel = semanticModel;
		}

		public int Calculate(TypeDeclarationSyntax type)
		{
			int num = 0;
			if (type.BaseList != null)
			{
				foreach (TypeSyntax syntax in type.BaseList.Types)
				{
					Func<TypeKind, bool> predicate = null;

					CommonSymbolInfo symbolInfo = _semanticModel.GetSymbolInfo(syntax);
					for (NamedTypeSymbol symbol = symbolInfo.Symbol as NamedTypeSymbol; symbol != null; symbol = symbol.BaseType)
					{
						if (predicate == null)
						{
							NamedTypeSymbol symbol1 = symbol;
							predicate = x => x == symbol1.TypeKind;
						}

						if (_inheritableTypes.Any(predicate))
						{
							num++;
						}
					}
				}
			}
			return num;
		}
	}
}