// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentComparer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
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
	using Roslyn.Services;

	internal class DocumentComparer : IEqualityComparer<IDocument>
	{
		private static readonly DocumentComparer InnerComparer = new DocumentComparer();

		private DocumentComparer()
		{
		}

		public static DocumentComparer Default
		{
			get { return InnerComparer; }
		}

		public bool Equals(IDocument x, IDocument y)
		{
			return x == null
					   ? y == null
					   : y != null && x.FilePath == y.FilePath;
		}

		public int GetHashCode(IDocument obj)
		{
			return obj.FilePath.GetHashCode();
		}
	}
}
