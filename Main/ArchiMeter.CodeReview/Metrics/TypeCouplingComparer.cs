// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCouplingComparer.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TypeCouplingComparer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMeter.CodeReview.Metrics
{
	using System.Collections.Generic;
	using Common.Metrics;

	internal class TypeCouplingComparer : IEqualityComparer<TypeCoupling>
	{
		private static readonly TypeCouplingComparer Instance = new TypeCouplingComparer();

		private TypeCouplingComparer()
		{
		}

		public static TypeCouplingComparer Default
		{
			get { return Instance; }
		}

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <returns>
		/// True if the specified objects are equal; otherwise, false.
		/// </returns>
		/// <param name="x">The first object of type <paramref name="T"/> to compare.</param><param name="y">The second object of type <paramref name="T"/> to compare.</param>
		public bool Equals(TypeCoupling x, TypeCoupling y)
		{
			return x == null
					   ? y == null
					   : y != null && y.ClassName == x.ClassName
						 && y.Namespace == x.Namespace
						 && y.Assembly == x.Assembly;
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <returns>
		/// A hash code for the specified object.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
		public int GetHashCode(TypeCoupling obj)
		{
			return (obj.ClassName + obj.Namespace + obj.Assembly).GetHashCode();
		}
	}
}