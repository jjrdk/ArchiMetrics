namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.Metrics;

	internal class Documentation : IDocumentation
	{
		public Documentation(string summary, string code, string example, string remarks, string returns, IEnumerable<ExceptionDescription> exceptions)
		{
			Summary = summary;
			Code = code;
			Example = example;
			Remarks = remarks;
			Returns = returns;
			Exceptions = exceptions.AsArray();
		}

		public string Summary { get; private set; }

		public string Code { get; private set; }

		public string Example { get; private set; }

		public string Remarks { get; private set; }

		public string Returns { get; private set; }

		public IEnumerable<ExceptionDescription> Exceptions { get; private set; }
	}
}