// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLoadErrorDocument.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LoadErrorDetailsDocument type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Common.Documents
{
	public class LoadErrorDetailsDocument
	{
		public string ProjectPath { get; set; }

		public string ErrorMessage { get; set; }

		public string StackTrace { get; set; }
	}

	public class ProjectLoadErrorDocument
	{
		public string Id { get; set; }

		public string ProjectName { get; set; }

		public string ProjectVersion { get; set; }

		public LoadErrorDetailsDocument[] Details { get; set; }

		public static string GetId(string projectName, string projectVersion)
		{
			return string.Format("LoadError.{0}.v{1}", projectName, projectVersion);
		}
	}
}
