// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INodeInspector.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the INodeInspector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Roslyn.Compilers.CSharp;

	public interface INodeInspector
	{
		Task<IEnumerable<EvaluationResult>> Inspect(string projectPath, SyntaxNode node);
	}
}
