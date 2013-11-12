// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlPropertyNode.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XamlPropertyNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Xaml
{
	public class XamlPropertyNode
	{
		public XamlPropertyNode(string propertyName, bool isDependency, object value)
		{
			Name = propertyName;
			Value = value;
			IsDependency = isDependency;
		}

		public string Name { get; private set; }

		public bool IsDependency { get; private set; }

		public object Value { get; private set; }
	}
}
