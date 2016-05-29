// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathExtensions.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the PathExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common
{
    using System.IO;

    public static class PathExtensions
	{
		public static string GetFileNameWithoutExtension(this string path)
		{
			var fileName = Path.GetFileNameWithoutExtension(path);

			return string.IsNullOrWhiteSpace(fileName) ? string.Empty : fileName;
		}

		public static string GetLowerCaseExtension(this string path)
		{
			var extension = Path.GetExtension(path);

			return string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.ToLowerInvariant();
		}

		public static string GetLowerCaseFullPath(this string path)
		{
			var fullPath = Path.GetFullPath(path);

			return string.IsNullOrWhiteSpace(fullPath) ? string.Empty : fullPath.ToLowerInvariant();
		}
	}
}