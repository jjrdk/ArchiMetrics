// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstalledPackages.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the InstalledPackages type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Support
{
	using System.Collections;
	using System.Collections.Generic;

	internal class InstalledPackages : IEnumerable<string>
	{
		private static readonly IList<string> PackageStrings = new[]
		{
			"Microsoft .NET Framework, version 4.5",
			"Autofac, version 3.3.0",
			"DotNetZip.Reduced, version 1.9.1.8",
			"GraphSharp, version 1.1.0.0",
			"Microsoft.Bcl.Immutable, version 1.1.20-beta",
			"Microsoft.Bcl.Metadata, version 1.0.9-alpha",
			"Microsoft.CodeAnalysis.CodeActions, version 0.6.4033103-beta",
			"Microsoft.CodeAnalysis.Common, version 0.6.4033103-beta",
			"Microsoft.CodeAnalysis.CSharp, version 0.6.4033103-beta",
			"Microsoft.CodeAnalysis.CSharp.Workspaces, version 0.6.4033103-beta",
			"Microsoft.CodeAnalysis.Workspaces.Common, version 0.6.4033103-beta",
			"ModernUI.WPF, version 1.0.5",
			"Newtonsoft.Json, version 6.0.1",
			"NHunspell, version 1.1.1",
			"QuickGraph, version 3.6.61119.7",
			"Rx-Core, version 2.2.2",
			"Rx-Interfaces, version 2.2.2",
			"Rx-Linq, version 2.2.2",
			"Rx-Main, version 2.2.2",
			"Rx-PlatformServices, version 2.2.3",
			"Rx-WPF, version 2.2.2",
			"Rx-Xaml, version 2.2.2",
			"WPFExtensions, version 1.0.0",
		};
		
		private static readonly InstalledPackages InnerInstance = new InstalledPackages();

		private InstalledPackages()
		{
		}

		public static InstalledPackages Instance
		{
			get { return InnerInstance; }
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<string> GetEnumerator()
		{
			return PackageStrings.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}