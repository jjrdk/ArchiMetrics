// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelEdge.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelEdge type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Support
{
	using System;
	using Common.Structure;
	using QuickGraph;

	public class ModelEdge : Edge<IModelNode>, IEquatable<ModelEdge>
	{
		public ModelEdge(IModelNode source, IModelNode target)
			: base(source, target)
		{
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// <code>true</code> if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(ModelEdge other)
		{
			return other != null
				   && other.Source.Equals(Source)
				   && other.Target.Equals(Target);
		}
	}
}