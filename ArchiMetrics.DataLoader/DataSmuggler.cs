// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSmuggler.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DataSmuggler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.DataLoader
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Common;
	using global::Raven.Abstractions.Data;
	using global::Raven.Abstractions.Smuggler;
	using global::Raven.Client;
	using global::Raven.Imports.Newtonsoft.Json;
	using global::Raven.Json.Linq;

	internal class DataSmuggler : SmugglerApiBase
	{
		private readonly IProvider<IDocumentStore> _storeProvider;
		private int _currentPage;

		public DataSmuggler(IProvider<IDocumentStore> storeProvider, SmugglerOptions smugglerOptions)
			: base(smugglerOptions)
		{
			_storeProvider = storeProvider;
		}

		protected override RavenJArray GetIndexes(int totalCount)
		{
			return new RavenJArray();
		}

		protected override RavenJArray GetDocuments(Guid lastEtag)
		{
			var documentStore = _storeProvider.Get();
			var jsonDocuments = documentStore.DatabaseCommands.GetDocuments(_currentPage, smugglerOptions.BatchSize);
			_currentPage += smugglerOptions.BatchSize;

			var array = new RavenJArray(jsonDocuments.Select(d => d.ToJson()));

			return array;
		}

		protected override Guid ExportAttachments(JsonTextWriter jsonWriter, Guid lastEtag)
		{
			return lastEtag;
		}

		protected override void PutIndex(string indexName, RavenJToken index)
		{
		}

		protected override void PutAttachment(AttachmentExportInfo attachmentExportInfo)
		{
		}

		protected override Guid FlushBatch(List<RavenJObject> batch)
		{
			return Guid.NewGuid();
		}

		protected override DatabaseStatistics GetStats()
		{
			return new DatabaseStatistics();
		}

		protected override void ShowProgress(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}

		protected override void EnsureDatabaseExists()
		{
		}
	}
}
