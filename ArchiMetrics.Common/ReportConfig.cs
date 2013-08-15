// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportConfig.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ReportConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common
{
	using System.Collections.ObjectModel;
	using System.Xml.Serialization;

	[XmlRoot("ReportConfig")]
	public class ReportConfig
	{
		public ReportConfig()
		{
		}

		public ReportConfig(string databaseUrl, string apiKey, string outputFile, string tfsConnectionString, string tfsServerUrl, Collection<ModelSettings> modelSettings, Collection<string> couplings)
		{
			DatabaseUrl = databaseUrl;
			ApiKey = apiKey;
			OutputFile = outputFile;
			TfsConnectionString = tfsConnectionString;
			TfsServerUrl = tfsServerUrl;
			Models = modelSettings;
			Couplings = couplings;
		}

		[XmlAttribute("TfsServerUrl")]
		public string TfsServerUrl { get; set; }

		[XmlAttribute("DatabaseUrl")]
		public string DatabaseUrl { get; set; }

		[XmlAttribute("ApiKey")]
		public string ApiKey { get; set; }

		[XmlAttribute("OutputFile")]
		public string OutputFile { get; set; }

		[XmlAttribute("TfsConnectionString")]
		public string TfsConnectionString { get; set; }

		[XmlElement("Project")]
		public virtual Collection<ProjectSettings> Projects { get; set; }

		[XmlElement("Model")]
		public Collection<ModelSettings> Models { get; set; }

		[XmlAttribute("Couplings")]
		public Collection<string> Couplings { get; set; }
	}
}