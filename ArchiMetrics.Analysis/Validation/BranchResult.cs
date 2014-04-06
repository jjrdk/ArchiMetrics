namespace ArchiMetrics.Analysis.Validation
{
	using ArchiMetrics.Common.Structure;

	internal class BranchResult : ValidationResultBase
	{
		public BranchResult(bool passed, IModelNode vertex)
			: base(passed, vertex)
		{
		}

		public override string Value
		{
			get
			{
				return Passed ? "Pattern not recognized." : "Pattern recognized.";
			}
		}
	}
}