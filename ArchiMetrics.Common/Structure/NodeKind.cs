namespace ArchiMetrics.Common.Structure
{
	using System.Linq;

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