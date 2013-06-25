// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSettingsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectSettingsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Reports.Tests
{
	using System;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Text;
	using System.Xml.Serialization;
	using Common;
	using NUnit.Framework;

	public class ProjectSettingsTests
	{
		[Test]
		public void CanSerializeProjectSettings()
		{
			const string Expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<ProjectSettings xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" Revision=""123"" Name=""Test Name"" Tag=""Test"" Date=""2013-05-04T16:13:00Z"">
  <Root Source=""abc"" IsTest=""false"" />
  <Root Source=""cde"" IsTest=""false"" />
  <Root Source=""fgh"" IsTest=""false"" />
  <TfsDefinition Definition=""TestDef"" />
</ProjectSettings>";

			var serializer = new XmlSerializer(typeof(ProjectSettings), new[] { typeof(ProjectDefinition) });
			var settings = new ProjectSettings
							   {
								   Name = "Test Name", 
								   Revision = "123", 
								   Tag = "Test",
								   Date = new DateTime(2013, 5, 4, 16, 13, 0, DateTimeKind.Utc),
								   Roots = new Collection<ProjectDefinition>
						                       {
							                       new ProjectDefinition { Source = "abc" }, 
							                       new ProjectDefinition { Source = "cde" }, 
							                       new ProjectDefinition { Source = "fgh" }
						                       },
											   TfsDefinitions = new Collection<TfsDefinition>
												                    {
													                    new TfsDefinition{Definition = "TestDef"}
												                    }
							   };
			var sb = new StringBuilder();
			serializer.Serialize(new StringWriter(sb), settings);
			Console.WriteLine(sb.ToString());

			Assert.AreEqual(Expected, sb.ToString());
		}
	}
}
