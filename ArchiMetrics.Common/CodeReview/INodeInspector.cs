// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeInspector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the INodeInspector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.CodeReview
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Compilers.Common;
	using Roslyn.Services;

	public interface INodeInspector
	{
		Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, SyntaxNode node, ISemanticModel semanticModel, ISolution solution);
	}
}
