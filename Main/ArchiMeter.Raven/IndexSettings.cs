// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexSettings.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IndexSettings type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.Raven
{
	public class IndexSettings
	{
		public IndexSettings()
			: this(string.Empty, false)
		{
		}

		public IndexSettings(string indexName, bool isMapReduce)
		{
			IndexName = indexName;
			IsMapReduce = isMapReduce;
		}

		public string IndexName { get; private set; }

		public bool IsMapReduce { get; private set; }
	}
}