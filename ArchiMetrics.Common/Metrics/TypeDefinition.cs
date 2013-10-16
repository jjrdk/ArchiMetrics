namespace ArchiMetrics.Common.Metrics
{
	using System;

	public class TypeDefinition : IComparable, IComparable<TypeDefinition>
	{
		private readonly string _fullName;

		public TypeDefinition(string className, string namespaceName, string assemblyName)
		{
			ClassName = className;
			Namespace = namespaceName;
			Assembly = assemblyName;

			_fullName = string.Format("{0}.{1}, {2}", namespaceName, className, assemblyName);
		}

		public string ClassName { get; private set; }

		public string Namespace { get; private set; }

		public string Assembly { get; private set; }

		public virtual int CompareTo(object obj)
		{
			var other = obj as TypeDefinition;
			return CompareTo(other);
		}

		public int CompareTo(TypeDefinition other)
		{
			return other == null
				? -1
				: string.Compare(_fullName, other._fullName, StringComparison.InvariantCultureIgnoreCase);
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