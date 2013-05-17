namespace ArchiMeter.Reports.Metrics
{
	using Roslyn.Compilers.Common;

	public abstract class SemanticModelMetricsCalculator
	{
		private readonly ISemanticModel _semanticModel;
		
		// Methods
		protected SemanticModelMetricsCalculator(ISemanticModel semanticModel)
		{
			_semanticModel = semanticModel;
		}

		// Properties
		protected ISemanticModel Model
		{
			get { return _semanticModel; }
		}

		protected CommonSyntaxNode Root
		{
			get { return _semanticModel.SyntaxTree.GetRoot(); }
		}
	}
}