// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCoupling.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCoupling type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class TypeCoupling : TypeDefinition
	{
		public TypeCoupling(string className, string namespaceName, string assemblyName, IEnumerable<string> usedMethods, IEnumerable<string> usedProperties, IEnumerable<string> useEvents)
			: base(className, namespaceName, assemblyName)
		{
			UsedMethods = usedMethods.ToArray();
			UsedProperties = usedProperties.ToArray();
			UsedEvents = useEvents.ToArray();
		}

		public string[] UsedMethods { get; private set; }

		public string[] UsedProperties { get; private set; }

		public string[] UsedEvents { get; private set; }
	}
}
