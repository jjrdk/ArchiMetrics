// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlNode.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XamlNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Xaml
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Xml;
	using System.Xml.Linq;

	public class XamlNode
	{
		private static readonly Regex TrueRegex = new Regex("^True$", RegexOptions.Compiled);
		private static readonly Regex FalseRegex = new Regex("^False$", RegexOptions.Compiled);

		public XamlNode(XamlNode parent, XElement xamlElement)
		{
			Parent = parent;
			ClassName = xamlElement.Name.LocalName;
			Children = new List<XamlNode>();
			Attributes = new List<XamlNodeAttribute>();
			Usings = new List<KeyValuePair<string, string>>
				         {
					         new KeyValuePair<string, string>(string.Empty, "System"), 
					         new KeyValuePair<string, string>(string.Empty, "System.Windows.Controls")
				         };
			ParseAttributes(xamlElement);
			Properties = ParseProperties(xamlElement);
			ParseChildren(xamlElement);
		}

		public IList<XamlNode> Children { get; private set; }

		public string BaseClassName { get; private set; }

		public string ClassName { get; private set; }

		public string NamespaceName { get; private set; }

		public IList<XamlNodeAttribute> Attributes { get; private set; }

		public IList<XamlPropertyNode> Properties { get; private set; }

		public IList<KeyValuePair<string, string>> Usings { get; private set; }

		public string VariableName { get; private set; }

		public XamlNode Parent { get; private set; }

		private void ParseAttributes(XElement node)
		{
			foreach (var attribute in node.Attributes())
			{
				if (attribute.Value.StartsWith("clr-namespace:"))
				{
					var ns = new KeyValuePair<string, string>(attribute.Name.LocalName, attribute.Value.Replace("clr-namespace:", string.Empty));
					Usings.Add(ns);
				}

				switch (attribute.Name.LocalName)
				{
					case "xmlns":
					case "x":
						break;
					case "Class":
						var components = attribute.Value.Split('.').ToArray();
						NamespaceName = string.Join(".", components.Take(components.Length - 1));
						BaseClassName = ClassName;
						ClassName = components.Last();
						break;
					case "Name":
						VariableName = attribute.Value;
						break;

					case "Content":
					case "Title":
						AddAttribute(attribute, "\"" + attribute.Value + "\"");
						break;

					case "Background":
					case "Stroke":
						{
							string extraCode = "SysColor col = ";
							if (attribute.Value.StartsWith("#"))
							{
								extraCode += "System.Drawing.ColorTranslator.FromHtml(\"" + attribute.Value + "\");";
							}
							else
							{
								extraCode += "SysColor." + attribute.Value + ";";
							}

							AddAttribute(attribute, "new SolidColorBrush(" + "Color.FromArgb(col.A, col.R, col.G, col.B))", extraCode);
						}

						break;

					case "Color":
						{
							string extraCode = "SysColor col = ";
							if (attribute.Value.StartsWith("#"))
							{
								extraCode += "System.Drawing.ColorTranslator.FromHtml(\"" + attribute.Value + "\");";
							}
							else
							{
								extraCode += "SysColor." + attribute.Value + ";";
							}

							AddAttribute(
								attribute, 
								"Color.FromArgb(col.A, col.R, col.G, col.B)", 
								extraCode);
						}

						break;

					case "HorizontalAlignment":
						AddAttribute(attribute, "HorizontalAlignment." + attribute.Value);
						break;

					case "VerticalAlignment":
						AddAttribute(attribute, "VerticalAlignment." + attribute.Value);
						break;

					case "HorizontalScrollBarVisibility":
					case "VerticalScrollBarVisibility":
						AddAttribute(attribute, "ScrollBarVisibility." + attribute.Value);
						break;

					case "Margin":
						AddAttribute(attribute, "new Thickness(" + attribute.Value + ")");
						break;

					case "EndPoint":
					case "StartPoint":
						AddAttribute(attribute, "new Point(" + attribute.Value + ")");
						break;
					case "Orientation":
						AddAttribute(attribute, "Orientation." + attribute.Value);
						break;
					default:
						AddAttribute(attribute, attribute.Value);
						break;
				}
			}

			if (string.IsNullOrEmpty(VariableName))
			{
				VariableName = VariableNameProvider.Get(ClassName);
			}
		}

		private IList<XamlPropertyNode> ParseProperties(XContainer node)
		{
			return node.Elements()
				.Where(e => e.Name.LocalName.Contains("."))
				.Select(element =>
							{
								var isDependency = element.Name.LocalName.Split('.').First() != ClassName;
								var value = element.HasElements
									            ? element.Elements()
									                     .ElementAtOrDefault(1) == null
										              ? (object)new XamlNode(
											                        null,
											                        element.Elements()
											                               .First())
										              : element.Elements()
										                       .Select(e => new XamlNode(null, e))
										                       .ToArray()
									            : element.Value;
								return new XamlPropertyNode(element.Name.LocalName.Split('.').Last(), isDependency, value);
							}).ToArray();
		}

		private void ParseChildren(XContainer currentXamlNode)
		{
			foreach (var node in currentXamlNode.Nodes())
			{
				switch (node.NodeType)
				{
					case XmlNodeType.Element:
						var element = (XElement)node;
						if (!element.Name.LocalName.Contains("."))
						{
							Children.Add(new XamlNode(this, element));
						}

						break;
					case XmlNodeType.Text:
						var text = (XText)node;
						if (text.NextNode != null)
						{
							Children.Add(new XamlNode(this, text.NextNode as XElement));
						}

						break;
				}
			}
		}

		private void AddAttribute(XAttribute attribute, string value, string extraCode = null)
		{
			value = TrueRegex.Replace(value, "true");
			value = FalseRegex.Replace(value, "false");
			Attributes.Add(new XamlNodeAttribute(attribute.Name.LocalName, value, extraCode));
		}
	}
}
