// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorData.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Documents
{
	public class ErrorData : ProjectDocument
	{
		public string Id { get; set; }

		public string Error { get; set; }

		public int DistinctLoc { get; set; }

		public int Occurrences { get; set; }

		public double Effort { get; set; }

		public string Category { get; set; }

		public static string GetId(string name, string projectName, string revision)
		{
			return string.Format("ErrorData.{0}.{1}.v{2}", name.Replace(" ", string.Empty), projectName, revision);
		}
	}
}