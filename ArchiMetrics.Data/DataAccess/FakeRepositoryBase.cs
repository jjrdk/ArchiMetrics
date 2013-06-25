// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeRepositoryBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FakeRepositoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Data.DataAccess
{
	using System.IO;

	public class FakeRepositoryBase
	{
		protected FakeRepositoryBase()
		{
		}

		protected Stream GetResourceStream(string resourceFile)
		{
			var assembly = typeof(FakeRepositoryBase).Assembly;
			var resourceName = string.Format("{0}.Data.{1}", assembly.GetName().Name, resourceFile);
			var info = assembly.GetManifestResourceStream(resourceName);

			return info;
		}
	}
}
