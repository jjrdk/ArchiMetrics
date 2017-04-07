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
    using System;
    using System.Globalization;
    using System.IO;

    public static class PathExtensions
	{
		public static string GetParentFolder(this string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return null;
			}

			if (File.Exists(path))
			{
				var directory = new FileInfo(path).Directory;
				return directory?.FullName ?? string.Empty;
			}

			var dir = new DirectoryInfo(path);

			return dir.Parent?.FullName ?? string.Empty;
		}

		public static string GetFullPath(this string path)
		{
			return Path.GetFullPath(path);
		}

		public static string CombineWith(this string path, string other)
		{
			return Path.Combine(path, other);
		}

		public static string GetFileNameWithoutExtension(this string path)
		{
			var fileName = Path.GetFileNameWithoutExtension(path);

			return string.IsNullOrWhiteSpace(fileName) ? string.Empty : fileName;
		}

		public static string ChangeExtension(this string path, string extension)
		{
			var fileName = Path.ChangeExtension(path, extension);

			return string.IsNullOrWhiteSpace(fileName) ? string.Empty : fileName;
		}

		public static string GetLowerCaseExtension(this string path)
		{
			var extension = Path.GetExtension(path);

			return string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.ToLowerInvariant();
		}

		public static string GetLowerCaseFullPath(this string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return string.Empty;
			}

			var fullPath = Path.GetFullPath(path);

			return string.IsNullOrWhiteSpace(fullPath) ? string.Empty : fullPath.ToLowerInvariant();
		}

		public static string GetPathRelativeTo(this string path, string other)
		{
			if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(other))
			{
				return string.Empty;
			}

			var pathUri = new Uri(path);

			if (!other.EndsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
			{
				other += Path.DirectorySeparatorChar;
			}

			var folderUri = new Uri(other);
			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}
	}
}