// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberDocumentation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IMemberDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System.Collections.Generic;

    public interface IMemberDocumentation : IDocumentation
	{
		IEnumerable<ParameterDocumentation> Parameters { get; }

		IEnumerable<TypeParameterDocumentation> TypeParameters { get; }

		IEnumerable<ExceptionDocumentation> Exceptions { get; }
	}
}