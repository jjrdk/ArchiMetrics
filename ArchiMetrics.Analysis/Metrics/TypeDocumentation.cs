// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDocumentation.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDocumentation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;

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