namespace ArchiMetrics.Common
{
	using System.Xml.Serialization;

	public class ModelSettings
	{
		[XmlAttribute("Name")]
		public string Name { get; set; }

		[XmlAttribute("Root")]
		public string Root { get; set; }

		[XmlAttribute("File")]
		public string File { get; set; }

		[XmlAttribute("DataFile")]
		public string Data { get; set; }
	}
}
