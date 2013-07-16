// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XamlNodeAttribute.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XamlNodeAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Common.Xaml
{
	public class XamlNodeAttribute
	{
		public XamlNodeAttribute(string variableName, string value, string extraCode)
		{
			VariableName = variableName;
			Value = value;
			ExtraCode = extraCode;
		}

		public string VariableName { get; private set; }

		public string Value { get; private set; }

		public string ExtraCode { get; private set; }
	}
}
