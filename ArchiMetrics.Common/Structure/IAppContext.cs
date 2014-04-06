namespace ArchiMetrics.Common.Structure
{
	using System;
	using System.ComponentModel;

	public interface IAppContext : INotifyPropertyChanged, IDisposable
	{
		string Path { get; set; }

		string RulesSource { get; set; }

		int MaxNamespaceDepth { get; set; }
	}
}
