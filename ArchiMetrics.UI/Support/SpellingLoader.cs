// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellingLoader.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SpellingLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Support
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;
	using System.Xml.XPath;
	using ArchiMetrics.Common;

	internal class SpellingLoader
	{
		public IEnumerable<string> Load(string filePath)
		{
			using (var sr = new StreamReader(File.OpenRead(filePath)))
			{
				var extension = filePath.GetLowerCaseExtension();
				switch (extension)
				{
					case ".spelling":
						return LoadStringList(sr);
					case ".xml":
						return LoadAnalysisDictionary(sr);
					default:
						return Enumerable.Empty<string>();
				}
			}
		}

		private static IEnumerable<string> LoadStringList(StreamReader sr)
		{
			var lines = new List<string>();
			while (sr.Peek() >= 0)
			{
				lines.Add(sr.ReadLine());
			}

			return lines;
		}

		private IEnumerable<string> LoadAnalysisDictionary(StreamReader sr)
		{
			try
			{
				var doc = XDocument.Load(sr);
				var root = doc.Root;
				if (root == null)
				{
					return Enumerable.Empty<string>();
				}

				var words = root.XPathSelectElements("//word");
				var compounds = root.XPathSelectElements("//term");
				var acronyms = root.XPathSelectElements("//Acronym");

				return words.Concat(compounds)
					.Concat(acronyms)
					.Select(x => x.Value)
					.AsArray();
			}
			catch
			{
				return Enumerable.Empty<string>();
			}
		}
	}
}
