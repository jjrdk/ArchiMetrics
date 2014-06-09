// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RepositoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System.IO;

	public class RepositoryBase
	{
		protected RepositoryBase()
		{
		}

		protected Stream GetResourceStream(string resourceFile)
		{
			var assembly = typeof(RepositoryBase).Assembly;
			var resourceName = string.Format("{0}.Data.{1}", assembly.GetName().Name, resourceFile);
			var info = assembly.GetManifestResourceStream(resourceName);
			
			return info;
		}
	}
}
