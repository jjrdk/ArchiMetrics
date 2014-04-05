namespace ArchiMetrics.Common.Tests
{
	using System.IO;
	using NUnit.Framework;

	public class SolutionProviderTests
	{
		private SolutionProvider _provider;

		[SetUp]
		public void Setup()
		{
			_provider = new SolutionProvider();
		}

		[Test]
		public void CanLoadSolutionFromPath()
		{
			var solutionPath = Path.GetFullPath(@"..\..\..\ArchiMetrics.sln");

			var solution = _provider.Get(solutionPath);

			Assert.NotNull(solution);
		}
	}
}
