// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal static class Extensions
    {
        private static readonly Regex CapitalRegex = new Regex("[A-Z]", RegexOptions.Compiled);
        private static readonly string[] KnownTestAttributes = { "Test", "TestCase", "TestMethod", "Fact", "Theory" };

        public static bool IsKnownTestAttribute(this string text)
        {
            return KnownTestAttributes.Contains(text);
        }

        public static void DisposeNotNull(this IDisposable disposable)
        {
            disposable?.Dispose();
        }

        public static string ToTitleCase(this string input)
        {
            return CapitalRegex.Replace(input, m => " " + m).Replace("_", " ").Trim();
        }

        public static async Task<T> FirstMatch<T>(this IEnumerable<Task<T>> tasks, Func<T, bool> predicate)
        {
            var taskArray = tasks.AsArray();
            var finished = await Task.WhenAny(taskArray).ConfigureAwait(false);
            if (predicate(finished.Result))
            {
                return finished.Result;
            }

            var remaining = taskArray.Except(new[] { finished }).AsArray();
            if (remaining.Length == 0)
            {
                return default(T);
            }

            return await FirstMatch(remaining, predicate).ConfigureAwait(false);
        }
    }
}
