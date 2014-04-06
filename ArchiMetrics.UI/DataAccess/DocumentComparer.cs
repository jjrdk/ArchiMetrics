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
