// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelEdgeItem.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelEdgeItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System;
	using Common.Structure;

    public class ModelEdgeItem : IEquatable<ModelEdgeItem>
	{
		public ModelEdgeItem(IModelNode source, IModelNode target)
		{
			Source = source;
			Target = target;
		}

		public IModelNode Source { get; private set; }

		public IModelNode Target { get; private set; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// <code>true</code> if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ModelEdgeItem other)
		{
			return other != null
				   && Source.Equals(other.Source)
				   && Target.Equals(other.Target);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return $"{Source}->{Target}";
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return (Source.QualifiedName + Target.QualifiedName).GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// <code>true</code> if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			return Equals(obj as ModelEdgeItem);
		}
	}
}
