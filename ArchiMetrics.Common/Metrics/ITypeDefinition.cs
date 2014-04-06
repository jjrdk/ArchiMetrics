namespace ArchiMetrics.Common.Metrics
{
	using System;

	public interface ITypeDefinition : IComparable<ITypeDefinition>
	{
		string TypeName { get; }

		string Namespace { get; }

		string Assembly { get; }
	}
}