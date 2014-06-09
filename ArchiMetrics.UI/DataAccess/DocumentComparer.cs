// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentComparer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System.Collections.Generic;
	using Microsoft.CodeAnalysis;

	internal class DocumentComparer : IEqualityComparer<Document>
	{
		private static readonly DocumentComparer InnerComparer = new DocumentComparer();

		private DocumentComparer()
		{
		}

		public static DocumentComparer Default
		{
			get { return InnerComparer; }
		}

		public bool Equals(Document x, Document y)
		{
			return x == null
					   ? y == null
					   : y != null && x.FilePath == y.FilePath;
		}

		public int GetHashCode(Document obj)
		{
			return obj.FilePath.GetHashCode();
		}
	}
}
