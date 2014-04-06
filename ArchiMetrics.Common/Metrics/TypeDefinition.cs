// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDefinition.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Metrics
{
	using System;

	internal class TypeDefinition : ITypeDefinition, IComparable, IComparable<ITypeDefinition>
	{
		private readonly string _fullName;

		public TypeDefinition(string typeName, string namespaceName, string assemblyName)
		{
			TypeName = typeName;
			Namespace = namespaceName;
			Assembly = assemblyName;

			_fullName = string.Format("{0}.{1}, {2}", namespaceName, typeName, assemblyName);
		}

		public string TypeName { get; private set; }

		public string Namespace { get; private set; }

		public string Assembly { get; private set; }

		public static bool operator ==(TypeDefinition c1, TypeDefinition c2)
		{
			return ReferenceEquals(c1, null)
					   ? ReferenceEquals(c2, null)
					   : c1.CompareTo(c2) == 0;
		}

		public static bool operator !=(TypeDefinition c1, TypeDefinition c2)
		{
			return ReferenceEquals(c1, null)
					   ? !ReferenceEquals(c2, null)
					   : c1.CompareTo(c2) != 0;
		}

		public static bool operator <(TypeDefinition c1, TypeDefinition c2)
		{
			return !ReferenceEquals(c1, null) && c1.CompareTo(c2) < 0;
		}

		public static bool operator >(TypeDefinition c1, TypeDefinition c2)
		{
			return !ReferenceEquals(c1, null) && c1.CompareTo(c2) > 0;
		}

		public virtual int CompareTo(object obj)
		{
			var other = obj as TypeDefinition;
			return CompareTo(other);
		}

		public int CompareTo(ITypeDefinition other)
		{
			return other == null
					   ? -1
					   : string.Compare(ToString(), other.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		public override string ToString()
		{
			return _fullName;
		}

		public override bool Equals(object obj)
		{
			return CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			return _fullName.GetHashCode();
		}
	}
}