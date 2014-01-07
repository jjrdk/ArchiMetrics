// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlConverter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XamlConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis
{
	using System.IO;
	using System.Xml.Linq;
	using ArchiMetrics.Common.Xaml;
	using Roslyn.Compilers.CSharp;

	internal class XamlConverter
	{
		private static readonly XamlCodeWriter CodeWriter = new XamlCodeWriter();

		public SyntaxTree ConvertSnippet(string snippet)
		{
			var doc = XDocument.Parse(snippet);
			var node = new XamlNode(null, doc.Root);
			return CodeWriter.CreateSyntax(node);
		}

		public SyntaxTree Convert(string filepath)
		{
			var text = File.ReadAllText(filepath);
			return ConvertSnippet(text);
		}
	}
}
