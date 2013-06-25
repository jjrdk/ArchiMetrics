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
namespace ArchiMetrics.Common.Documents
{
	public class EvaluationResultDocument : ProjectDocument
	{
		public string Id { get; set; }
		
		public EvaluationResult[] Results { get; set; }

		public static string GetId(string projectName, string revision)
		{
			return string.Format("Errors.{0}.v{1}", projectName, revision);
		}
	}
}
