namespace ArchiMeter.Common.Tests
{
	using System.IO;
	using NUnit.Framework;
	using Roslyn.Services;

	public class RoslynExtensionTests
	{
		[Test]
		[Ignore("Need to understand what global section is missing.")]
		public void CanSaveSolution()
		{
			var solution = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiMeter.sln")).CurrentSolution;
			solution.Save(@"..\..\..\x.sln", true);
		}

		[Test]
		[Ignore("Need to understand what global section is missing.")]
		public void CanMergeSolutions()
		{
			var main = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiMeter.sln")).CurrentSolution;
			var other = Workspace.LoadSolution(Path.GetFullPath(@"..\..\..\ArchiCop.sln")).CurrentSolution;
			var merged = main.MergeWith(other);

			Assert.NotNull(merged);
		}

		[Test]
		[Ignore("Need to understand what global section is missing.")]
		public void CanMergeSolutions2()
		{
			RoslynExtensions.MergeSolutionsTo(
				@"C:\Dev\Tfs\NewGen.Dev\Units\NewGen.sln",
				@"C:\Dev\Tfs\NewGen.Dev\Units\CWAM\CWAM.sln",
				@"C:\Dev\Tfs\NewGen.Dev\Units\IM\IM.sln",
				@"C:\Dev\Tfs\NewGen.Dev\Units\UI\UI.sln");
		}
	}
}