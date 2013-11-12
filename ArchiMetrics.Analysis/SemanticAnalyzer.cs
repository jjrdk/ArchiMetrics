// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SemanticAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using Roslyn.Compilers.Common;
	using Roslyn.Compilers.CSharp;

	public class SemanticAnalyzer
	{
		private readonly ISemanticModel _model;

		public SemanticAnalyzer(ISemanticModel model)
		{
			_model = model;
		}

		public IEnumerable<ParameterSyntax> GetUnusedParameters(BaseMethodDeclarationSyntax method)
		{
			if (method.ParameterList.Parameters.Count == 0)
			{
				return new ParameterSyntax[0];
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());

			var usedParameterNames = dataflow.DataFlowsIn
				.Where(x => x.Kind == CommonSymbolKind.Parameter)
				.Select(x => x.Name)
				.ToArray();

			var unusedParameters = method.ParameterList.Parameters
				.Where(p => !usedParameterNames.Contains(p.Identifier.ValueText))
				.ToArray();
			return unusedParameters;
		}

		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "TypeDeclarationSyntax intended.")]
		public IEnumerable<MethodDeclarationSyntax> GetPossibleStaticMethods(TypeDeclarationSyntax type)
		{
			return type.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Where(x => !x.Modifiers.Any(SyntaxKind.StaticKeyword))
				.Where(CanBeMadeStatic)
				.ToArray();
		}

		public bool CanBeMadeStatic(BaseMethodDeclarationSyntax method)
		{
			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());
			var hasThisReference = dataflow.DataFlowsIn
				.Any(x => x.Kind == CommonSymbolKind.Parameter && x.Name == Syntax.Token(SyntaxKind.ThisKeyword).ToFullString());
			return !hasThisReference;
		}
	}
}