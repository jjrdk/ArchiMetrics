// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeErrors.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the CodeErrors type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Common.Documents
{
	public class CodeErrors : ProjectDocument
	{
		public string Namespace { get; set; }

		public string Error { get; set; }

		public CodeSnippet[] Snippets { get; set; }
	}

	public class CodeSnippet
	{
		public string FilePath { get; set; }

		public string Snippet { get; set; }
	}
}