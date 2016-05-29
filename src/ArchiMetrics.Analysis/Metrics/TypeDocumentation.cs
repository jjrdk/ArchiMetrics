// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDocumentation.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using Common;
	using Common.Metrics;

    internal class TypeDocumentation : ITypeDocumentation
	{
		public TypeDocumentation(string summary, string code, string example, string remarks, string returns, IEnumerable<TypeParameterDocumentation> typeParameters)
		{
			Summary = summary;
			Code = code;
			Example = example;
			Remarks = remarks;
			Returns = returns;
			TypeParameters = typeParameters.AsArray();
		}

		public string Summary { get; private set; }

		public string Code { get; private set; }

		public string Example { get; private set; }

		public string Remarks { get; private set; }

		public string Returns { get; private set; }

		public IEnumerable<TypeParameterDocumentation> TypeParameters { get; private set; }
	}
}