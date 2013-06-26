namespace ArchiMetrics.Raven.Tests.Indexes
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using global::Raven.Imports.Newtonsoft.Json;
	using NUnit.Framework;

	[SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Type parameters required.")]
	public abstract class IndexTestBase<TDocument, TReduce, TIndex>
		where TIndex : ITestIndex<TDocument, TReduce>, new()
	{
		protected TIndex Index { get; private set; }

		[SetUp]
		public void Setup()
		{
			Index = new TIndex();
			AdditionalSetup();
		}

		protected virtual void AdditionalSetup()
		{
		}

		protected object[] PerformMapReduce(IEnumerable<TDocument> docs)
		{
			var mapped = Index.GetMap().Compile()(docs);
			var json = JsonConvert.SerializeObject(mapped);
			var errors = JsonConvert.DeserializeObject<TReduce[]>(json);
			var reduced = Index.GetReduce().Compile()(errors);
			var results = reduced.OfType<object>().ToArray();
			return results;
		}
	}
}
