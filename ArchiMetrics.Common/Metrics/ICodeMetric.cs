// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeMetric.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ICodeMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System.Collections.Generic;

	/// <summary>
	/// Defines the base interface for types providing code metric values.
	/// </summary>
	public interface ICodeMetric
	{
		/// <summary>
		/// Gets the type couplings for the members.
		/// </summary>
		IEnumerable<ITypeCoupling> ClassCouplings { get; }

		/// <summary>
		/// Gets the lines of code.
		/// </summary>
		/// <remarks>For a description of how lines of code are counted consult this article: http://blogs.msdn.com/b/zainnab/archive/2011/05/12/code-metrics-lines-of-code.aspx .</remarks>
		int LinesOfCode { get; }

		/// <summary>
		/// Gets the maintainability index.
		/// </summary>
		/// <remarks>For a description of how the maintainability is calculated consult this article: http://blogs.msdn.com/b/zainnab/archive/2011/05/26/code-metrics-maintainability-index.aspx .</remarks>
		double MaintainabilityIndex { get; }

		/// <summary>
		/// Gets the cyclomatic complexity.
		/// </summary>
		/// <remarks>For a description of how is calculated consult this article: http://blogs.msdn.com/b/zainnab/archive/2011/05/12/code-metrics-lines-of-code.aspx .</remarks>
		int CyclomaticComplexity { get; }

		/// <summary>
		/// Gets the name of the instance the metrics are related to.
		/// </summary>
		string Name { get; }
	}
}