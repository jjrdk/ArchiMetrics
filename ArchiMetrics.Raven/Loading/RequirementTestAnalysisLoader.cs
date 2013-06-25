// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTestAnalysisLoader.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   RequirementTestAnalysisLoader.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Raven.Loading
{
	// public class RequirementTestAnalysisLoader : IDataLoader
	// {
	// 	private readonly IRequirementTestAnalyzer _analyzer;

	// 	public RequirementTestAnalysisLoader(IRequirementTestAnalyzer analyzer)
	// 	{
	// 		_analyzer = analyzer;
	// 	}

	// 	public Task Load(ProjectSettings settings)
	// 	{
	// 		var requirementTests = settings.Roots.SelectMany(root => _analyzer.GetRequirementTests(root.Source));
	// 		//requirementTests.Select(test=>test.)
	// 	}

	// 	public void Dispose()
	// 	{
	// 		Dispose(true);
	// 		GC.SuppressFinalize(this);
	// 	}

	// 	protected virtual void Dispose(bool isDisposing)
	// 	{
	// 		if (isDisposing)
	// 		{
	// 			//Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.

	// 		}
	// 	}

	// 	~RequirementTestAnalysisLoader()
	// 	{
	// 		// Simply call Dispose(false).
	// 		Dispose(false);
	// 	}

	// }
}
