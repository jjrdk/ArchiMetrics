// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberNode.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.Common.Metrics
{
	using Roslyn.Compilers.Common;

	public class MemberNode
	{
		public MemberNode(string codeFile, string displayName, MemberKind kind, int lineNumber, CommonSyntaxNode syntaxNode)
		{
			CodeFile = codeFile;
			DisplayName = displayName;
			Kind = kind;
			LineNumber = lineNumber;
			SyntaxNode = syntaxNode;
		}

		public string CodeFile { get; set; }

		public string DisplayName { get; set; }

		public MemberKind Kind { get; set; }

		public int LineNumber { get; set; }

		public CommonSyntaxNode SyntaxNode { get; set; }
	}
}
