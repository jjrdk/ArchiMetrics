// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectComparer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common
{
	using System.Collections.Generic;
	using Roslyn.Services;

	public class ProjectComparer : IEqualityComparer<IProject>
	{
		private static readonly ProjectComparer InnerComparer = new ProjectComparer();

		private ProjectComparer()
		{
		}

		public static ProjectComparer Default
		{
			get { return InnerComparer; }
		}

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <returns>
		/// True if the specified objects are equal; otherwise, false.
		/// </returns>
		/// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
		/// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
		public bool Equals(IProject x, IProject y)
		{
			return x == null
					   ? y == null
					   : y != null && x.FilePath == y.FilePath;
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <returns>
		/// A hash code for the specified object.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
		public int GetHashCode(IProject obj)
		{
			return obj == null ? 0 : obj.FilePath.GetHashCode();
		}
	}
}
