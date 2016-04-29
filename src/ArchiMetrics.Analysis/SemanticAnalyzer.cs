// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticAnalyzer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
	using Common;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	public class SemanticAnalyzer
	{
		private readonly SemanticModel _model;

		public SemanticAnalyzer(SemanticModel model)
		{
			_model = model;
		}

		public IEnumerable<ParameterSyntax> GetUnusedParameters(BaseMethodDeclarationSyntax method)
		{
			if (method.ParameterList.Parameters.Count == 0 || method.Body == null || !method.Body.ChildNodes().Any())
			{
				return new ParameterSyntax[0];
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());

			var usedParameterNames = dataflow.DataFlowsIn
				.Where(x => x.Kind == SymbolKind.Parameter)
				.Select(x => x.Name)
				.AsArray();

			var unusedParameters = method.ParameterList.Parameters
				.Where(p => !usedParameterNames.Contains(p.Identifier.ValueText))
				.AsArray();
			return unusedParameters;
		}

		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "TypeDeclaration constraint intended.")]
		public IEnumerable<MethodDeclarationSyntax> GetPossibleStaticMethods(TypeDeclarationSyntax type)
		{
			return type.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Where(x => !x.Modifiers.Any(SyntaxKind.StaticKeyword))
				.Where(CanBeMadeStatic)
				.AsArray();
		}

		public bool CanBeMadeStatic(BaseMethodDeclarationSyntax method)
		{
			if (method.Modifiers.Any(SyntaxKind.StaticKeyword)
				|| method.Body == null
				|| !method.Body.ChildNodes().Any())
			{
				return false;
			}

			var bodyNodes = method.Body.ChildNodes();
			var dataflow = _model.AnalyzeDataFlow(bodyNodes.First(), bodyNodes.Last());
			var hasThisReference = dataflow.DataFlowsIn
				.Any(x => x.Kind == SymbolKind.Parameter && x.Name == SyntaxFactory.Token(SyntaxKind.ThisKeyword).ToFullString());
			return !hasThisReference;
		}
	}
}