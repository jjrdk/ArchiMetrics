// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeDocumentation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common.Metrics
{
    using System.Collections.Generic;

    public interface ITypeDocumentation : IDocumentation
	{
		IEnumerable<TypeParameterDocumentation> TypeParameters { get; } 
	}
}