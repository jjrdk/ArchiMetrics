// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeKind.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NodeKind type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Model
{
	using System.Linq;
	using Common.Structure;

    public static class NodeKind
	{
		private static readonly string[] SharedNodeKinds = { Undefined, Solution, Assembly, Namespace };

		public static string Undefined
		{
			get { return "undefined"; }
		}

		public static string Solution
		{
			get { return "Solution"; }
		}

		public static string Assembly
		{
			get { return "Assembly"; }
		}

		public static string Namespace
		{
			get { return "Namespace"; }
		}

		public static string Class
		{
			get { return "Class"; }
		}

		public static string Struct
		{
			get { return "Struct"; }
		}

		public static string Member
		{
			get { return "Member"; }
		}

		public static bool IsShared(this IModelNode vertex)
		{
			return string.IsNullOrWhiteSpace(vertex.Type) || SharedNodeKinds.Contains(vertex.Type);
		}
	}
}