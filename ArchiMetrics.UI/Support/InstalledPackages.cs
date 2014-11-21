// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstalledPackages.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
			"Autofac, version 3.5.2",
			"DotNetZip.Reduced, version 1.9.1.8",
			"GraphSharp, version 1.1.0.0",
			"Microsoft.CodeAnalysis.CodeActions, version 0.7.4091001-beta",
			"Microsoft.CodeAnalysis.Common, version 1.0.0-beta1-20141031-01",
			"Microsoft.CodeAnalysis.CSharp, version 1.0.0-beta1-20141031-01",
			"Microsoft.CodeAnalysis.CSharp.Workspaces, version 1.0.0-beta1-20141031-01",
			"Microsoft.CodeAnalysis.Workspaces.Common, version 1.0.0-beta1-20141031-01",
			"Microsoft.Composition, version 1.0.27",
			"ModernUI.WPF, version 1.0.6",
			"Newtonsoft.Json, version 6.0.6",
			"NHunspell, version 1.2.5359.26126",
			"QuickGraph, version 3.6.61119.7",
			"Rx-Core, version 2.2.5",
			"Rx-Interfaces, version 2.2.5",
			"Rx-Linq, version 2.2.5",
			"Rx-Main, version 2.2.5",
			"Rx-PlatformServices, version 2.2.5",
			"Rx-WPF, version 2.2.5",
			"Rx-Xaml, version 2.2.5",
			"System.Collections.Immutable, version 1.1.32-beta",
			"System.Reflection.Metadata, version 1.0.17-beta",
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