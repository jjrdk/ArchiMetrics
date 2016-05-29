// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeMetric.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeMetric type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System.Collections.Generic;

    public interface ITypeMetric : ICodeMetric
	{
		AccessModifierKind AccessModifier { get; }

		TypeMetricKind Kind { get; }

		IEnumerable<IMemberMetric> MemberMetrics { get; }

		int DepthOfInheritance { get; }

		int ClassCoupling { get; }
		
		int AfferentCoupling { get; }

		int EfferentCoupling { get; }

		double Instability { get; }

		bool IsAbstract { get; }

		ITypeDocumentation Documentation { get; }
	}
}