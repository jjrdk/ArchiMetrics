// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvaluationResultDocument.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EvaluationResultDocument type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Documents
{
	public class EvaluationResultDocument
	{
		public string Id { get; set; }
		
		public string ProjectName { get; set; }
		
		public string ProjectVersion { get; set; }

		public EvaluationResult[] Results { get; set; }

		public static string GetId(string projectName, string revision)
		{
			return string.Format("Errors.{0}.v{1}", projectName, revision);
		}
	}

	// public class DatabaseContext : IDisposable
	// {
	// 	public DatabaseContext()
	// 	{
	// 		var repo = new ProductRepository(null);
	// 		var x = repo.ReadProducts();
	// 	}

	// 	public IEnumerable<string> Products { get; set; } 
	// }

	// public class ProductRepository
	// {
	// 	private readonly Func<DatabaseContext> contextCreator;

	// 	public ProductRepository(Func<DatabaseContext> contextCreator)
	// 	{
	// 		contextCreator = contextCreator;
	// 	}

	// 	public IEnumerable<string> ReadProducts()
	// 	{
	// 		using (DatabaseContext context = contextCreator())
	// 		{
	// 			return context.Products.ToArray();
	// 		}
	// 	}
	// }
}