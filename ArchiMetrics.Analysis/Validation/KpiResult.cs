namespace ArchiMetrics.Analysis.Validation
{
	using ArchiMetrics.Common.Structure;

	internal class KpiResult : ValidationResultBase
	{
		public KpiResult(bool passed, IModelNode vertex)
			: base(passed, vertex)
		{
		}

		public override string Value
		{
			get
			{
				return "Found node outside quality range";
			}
		}
	}
}