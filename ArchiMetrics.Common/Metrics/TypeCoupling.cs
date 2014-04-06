namespace ArchiMetrics.Common.Metrics
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class TypeCoupling : TypeDefinition, ITypeCoupling
	{
		public TypeCoupling(string typeName, string namespaceName, string assemblyName, IEnumerable<string> usedMethods, IEnumerable<string> usedProperties, IEnumerable<string> useEvents)
			: base(typeName, namespaceName, assemblyName)
		{
			UsedMethods = usedMethods.Distinct().ToArray();
			UsedProperties = usedProperties.Distinct().ToArray();
			UsedEvents = useEvents.Distinct().ToArray();
		}

		public IEnumerable<string> UsedMethods { get; private set; }

		public IEnumerable<string> UsedProperties { get; private set; }

		public IEnumerable<string> UsedEvents { get; private set; }

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(ITypeCoupling other)
		{
			return other == null ? -1 : string.Compare(ToString(), other.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
