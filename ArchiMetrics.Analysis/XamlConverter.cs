namespace ArchiMetrics.Analysis
{
	using System.IO;
	using System.Xml.Linq;
	using Common.Xaml;
	using Roslyn.Compilers.CSharp;

	public class XamlConverter
	{
		private static readonly XamlCodeWriter CodeWriter = new XamlCodeWriter();

		private string GetNodeName(string propertyName,
		                           XElement currentXamlNode)
		{
			return string.IsNullOrWhiteSpace(propertyName) 
				? currentXamlNode.Name.LocalName 
				: propertyName;
		}

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
