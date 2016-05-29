// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelNode.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ModelNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Collections.Generic;
	using Common.CodeReview;
	using Common.Structure;

    public class ModelNode : IModelNode
	{
		private readonly IList<IModelNode> _children;

		public ModelNode(string name, string type, CodeQuality quality, int linesOfCode, double maintainabilityIndex, int cyclomaticComplexity)
			: this(name, type, quality, linesOfCode, maintainabilityIndex, cyclomaticComplexity, new List<IModelNode>())
		{
		}

		public ModelNode(string name, string type, CodeQuality quality, int linesOfCode, double maintainabilityIndex, int cyclomaticComplexity, IList<IModelNode> vertices)
		{
			DisplayName = name;
			Quality = quality;
			LinesOfCode = linesOfCode;
			MaintainabilityIndex = maintainabilityIndex;
			CyclomaticComplexity = cyclomaticComplexity;
			QualifiedName = name;
			Type = type ?? NodeKind.Undefined;
			_children = vertices;
			foreach (var child in Children)
			{
				child.SetParent(this);
			}
		}

		public IModelNode Parent { get; protected set; }

		public string DisplayName { get; private set; }

		public CodeQuality Quality { get; private set; }

		public int LinesOfCode { get; private set; }

		public double MaintainabilityIndex { get; private set; }

		public int CyclomaticComplexity { get; private set; }

		public string QualifiedName { get; private set; }

		public string Type { get; private set; }

		public IEnumerable<IModelNode> Children
		{
			get
			{
				return _children;
			}
		}

		public static bool operator ==(ModelNode first, ModelNode second)
		{
			return first != null && first.Equals(second);
		}

		public static bool operator !=(ModelNode first, ModelNode second)
		{
			return first == null || !first.Equals(second);
		}

		public virtual void SetParent(IModelNode parent)
		{
			Parent = parent;
			QualifiedName = GetQualifiedName();
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// <code>true</code> if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(IModelNode other)
		{
			var isEqual = other != null
				   && QualifiedName.Equals(other.QualifiedName)
				   && Type.Equals(other.Type);

			return isEqual;
		}

		public IEnumerable<IModelNode> Flatten()
		{
			yield return this;
			foreach (var vertex in FlattenChildren(Children))
			{
				yield return vertex;
			}
		}

		public void AddChild(IModelNode child)
		{
			child.SetParent(this);
			_children.Add(child);
		}

		public void RemoveChild(IModelNode child)
		{
			if (_children.Remove(child))
			{
				child.SetParent(null);
			}
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return DisplayName.GetHashCode();
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
			return Equals(obj as IModelNode);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0} ({1})", QualifiedName, _children.Count);
		}

		private string GetQualifiedName()
		{
			return Parent == null || this.IsShared()
				? DisplayName
				: string.Format("{0}.{1}", Parent.QualifiedName, DisplayName);
		}

		private IEnumerable<IModelNode> FlattenChildren(IEnumerable<IModelNode> vertices)
		{
			foreach (var node in vertices)
			{
				yield return node;
				if (node != null)
				{
					foreach (var child in FlattenChildren(node.Children))
					{
						yield return child;
					}
				}
			}
		}
	}
}
