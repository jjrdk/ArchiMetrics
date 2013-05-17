// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelComparisonReport.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   ModelComparisonReport.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Reports
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMeter.Common;

	using OfficeOpenXml;

	 //public class ModelComparisonReport : IReportJob
	 //{
	 //   private readonly IProvider<IProject> _projectProvider;
	 //   private Task _compareTask;
	 //   private readonly Func<IComparisonResult, bool> _testMemberFilter = r => !r.Properties.Any(p => p.ReturnType.StartsWith("Mock", StringComparison.OrdinalIgnoreCase) || p.Name.Contains("_")) && !r.Methods.Any(m => m.ReturnType.StartsWith("Mock", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("_"));
	 //   private readonly Func<string, bool> _testCodeFilter = x => !x.StartsWith("Fake") && !x.StartsWith("Dummy") && !x.Contains("Mock") && !x.Contains(" ") && !x.EndsWith("Test") && !x.EndsWith("Tests") && !x.Contains("_");

	 //   public ModelComparisonReport(IProvider<IProject> projectProvider)
	 //   {
	 //	   _projectProvider = projectProvider;
	 //   }

	 //   public void AddReport(ExcelPackage package, ReportConfig config)
	 //   {
	 //	   var worksheet = package.Workbook.Worksheets.Add(string.Format("Model to Code Comparison {0}", ReportUtils.GetMonth()));
	 //	   worksheet.Cells[1, 1].Value = "Implementation";
	 //	   worksheet.Cells[2, 1].Value = "Implemented Classes";
	 //	   worksheet.Cells[3, 1].Value = "Implemented Interfaces";
	 //	   worksheet.Cells[4, 1].Value = "Classes Only In Model";
	 //	   worksheet.Cells[5, 1].Value = "Classes Only In Code";
	 //	   worksheet.Cells[6, 1].Value = "Classes In Both";
	 //	   worksheet.Cells[7, 1].Value = "Interfaces Only In Model";
	 //	   worksheet.Cells[8, 1].Value = "Interfaces Only In Code";
	 //	   worksheet.Cells[9, 1].Value = "Interfaces In Both";

	 //	   var tasks =
	 //		   (from model in
	 //				config.Models.Where(
	 //					m =>
	 //					!string.IsNullOrWhiteSpace(m.Name) && !string.IsNullOrWhiteSpace(m.File)
	 //					&& config.Projects.Any(p => p.Name == m.Name))
	 //			let proxy = new EnterpriseArchitectProxy(model.File)
	 //			let comparer =
	 //				new ModelToCodeComparer(proxy.LogicalModels.Where(p => string.IsNullOrWhiteSpace(model.Root) || model.Root == p.Name), model)
	 //			let codeRoot = config.Projects.First(p => p.Name == model.Name)
	 //			let solutionNodes = GetSolutionNodes(codeRoot.Root)
	 //			let typeDeclarationSyntaxs = solutionNodes.SelectMany(n => n.DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>().Select(t => t.Identifier.ValueText)).ToArray()
	 //			let modelName = model.Name
	 //			select ((IModelToCodeComparer)comparer).CompareAsync(solutionNodes)
	 //												   .ContinueWith(t => new KeyValuePair<string, IEnumerable<IComparisonResult>>(
	 //																		  modelName,
	 //																		  t.Result.Where(
	 //																			  c =>
	 //																			  !c.Name.Contains(" ")
	 //																			  && (c.Implementation == ImplementationType.OnlyInModel
	 //																				  || solutionNodes.Any(n => typeDeclarationSyntaxs.Any(s => string.Equals(s, c.Name, StringComparison.OrdinalIgnoreCase))))))))
	 //			   .ToArray();

	 //	   _compareTask = Task.Factory.ContinueWhenAll(tasks, t => WriteWorksheet(worksheet, t.Select(x => x.Result)));
	 //	   _compareTask.Wait();
	 //   }

	 //   public void Dispose()
	 //   {
	 //	   Dispose(true);
	 //	   GC.SuppressFinalize(this);
	 //   }

	 //   protected virtual void Dispose(bool isDisposing)
	 //   {
	 //	   if (isDisposing)
	 //	   {
	 //		   if (_compareTask != null)
	 //		   {
	 //			   _compareTask.Dispose();
	 //		   }
	 //	   }
	 //   }

	 //   ~ModelComparisonReport()
	 //   {
	 //	   // Simply call Dispose(false).
	 //	   Dispose(false);
	 //   }

	 //   private void WriteWorksheet(ExcelWorksheet worksheet, IEnumerable<KeyValuePair<string, IEnumerable<IComparisonResult>>> results)
	 //   {
	 //	   var allResults = results.ToArray();
	 //	   for (var i = 0; i < allResults.Length; i++)
	 //	   {
	 //		   var resultName = allResults[i].Key;
	 //		   var resultValues = allResults[i].Value.ToArray();
	 //		   var classesInBoth = GetNames(resultValues, ImplementationType.InBoth, ElementType.Class);
	 //		   var classesOnlyInCode = GetNames(resultValues, ImplementationType.OnlyInCode, ElementType.Class);
	 //		   var classesOnlyInModel = GetNames(resultValues, ImplementationType.OnlyInModel, ElementType.Class).Where(x => !x.StartsWith("System.")).ToArray();
	 //		   var interfacesInBoth = GetNames(resultValues, ImplementationType.InBoth, ElementType.Interface);
	 //		   var interfacesOnlyInCode = GetNames(resultValues, ImplementationType.OnlyInCode, ElementType.Interface);
	 //		   var interfacesOnlyInModel = GetNames(resultValues, ImplementationType.OnlyInModel, ElementType.Interface).Where(x => !x.StartsWith("System.")).ToArray();
	 //		   int col = i + 2;
	 //		   worksheet.Cells[1, col].Value = resultName;
	 //		   worksheet.Cells[2, col].Value = classesInBoth.Length + classesOnlyInCode.Length;
	 //		   worksheet.Cells[3, col].Value = interfacesInBoth.Length + interfacesOnlyInCode.Length;
	 //		   worksheet.Cells[4, col].Value = classesOnlyInModel.Length;
	 //		   worksheet.Cells[5, col].Value = classesOnlyInCode.Length;
	 //		   worksheet.Cells[6, col].Value = classesInBoth.Length;
	 //		   worksheet.Cells[7, col].Value = interfacesOnlyInModel.Length;
	 //		   worksheet.Cells[8, col].Value = interfacesOnlyInCode.Length;
	 //		   worksheet.Cells[9, col].Value = interfacesInBoth.Length;
	 //	   }

	 //   }

	 //   private string[] GetNames(IEnumerable<IComparisonResult> resultValues, ImplementationType implementationType, ElementType elementType)
	 //   {
	 //	   return resultValues.Where(r => r.Implementation == implementationType)
	 //						  .Where(r => _testMemberFilter(r) && r.Type == elementType)
	 //						  .Select(r => r.Name).Where(_testCodeFilter)
	 //						   .Distinct()
	 //						   .OrderBy(x => x)
	 //						   .ToArray();
	 //   }

	 //   private IEnumerable<SyntaxNode> GetSolutionNodes(string root)
	 //   {
	 //	   return Directory.GetFiles(root, "*.csproj", SearchOption.AllDirectories)
	 //			   .Where(ReportUtils.AllCode)
	 //			   .Select(_projectProvider.Get)
	 //			   .Where(p => p != null)
	 //			   .Distinct(new ProjectComparer())
	 //			   .SelectMany(p => p.Documents)
	 //			   .Distinct(new DocumentComparer())
	 //			   .Select(d => d.GetSyntaxTree().GetRoot() as SyntaxNode)
	 //			   .Where(n => n != null)
	 //			   .ToArray()
	 //			   .AsEnumerable();
	 //   }
	 //}
}
