// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exporter.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Exporter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.DataLoader
{
	using System;
	using System.IO;
	using System.Text;
	using Common;
	using global::Raven.Abstractions.Smuggler;
	using global::Raven.Client;
	using Ionic.Zip;

	internal class Exporter
	{
		private const string BackupFile = @"Data.Backup.ravendump";
		private readonly ReportConfig _config;
		private readonly SmugglerApiBase _smuggler;
		private readonly SmugglerOptions _smugglerOptions;

		public Exporter(ReportConfig config, IProvider<IDocumentStore> documentStoreProvider)
		{
			_config = config;
			_smugglerOptions = new SmugglerOptions
							   {
								   BackupPath = BackupFile, 
								   OperateOnTypes = ItemType.Documents
							   };
			_smuggler = new DataSmuggler(documentStoreProvider, _smugglerOptions);
		}

		public void Export()
		{
			_smuggler.ExportData(_smugglerOptions);
			if (File.Exists(_config.OutputFile))
			{
				File.Delete(_config.OutputFile);
			}

			using (var zipFile = new ZipFile(_config.OutputFile, Console.Out, Encoding.UTF8))
			{
				zipFile.AddFile(BackupFile);
				zipFile.Save();
			}

			File.Delete(BackupFile);
		}
	}
}
