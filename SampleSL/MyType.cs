// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyType.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MyType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SampleSL
{
	using System;

	public class MyType
	{
		public string Value { get; set; }

		public void DoSomething()
		{
			try
			{
				var x = 1 + 2;
				var y = x + 2;
			}
			catch
			{
				throw new Exception();
			}
		}
	}
}
