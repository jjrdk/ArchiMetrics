// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberNode.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
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
		public MemberNode(string codeFile, string displayName, MemberKind kind, int lineNumber, CommonSyntaxNode syntaxNode, ISemanticModel semanticModel)
		{
			CodeFile = codeFile;
			DisplayName = displayName;
			Kind = kind;
			LineNumber = lineNumber;
			SyntaxNode = syntaxNode;
			SemanticModel = semanticModel;
		}

		public string CodeFile { get; private set; }

		public string DisplayName { get; private set; }

		public MemberKind Kind { get; private set; }

		public int LineNumber { get; private set; }

		public CommonSyntaxNode SyntaxNode { get; private set; }

		public ISemanticModel SemanticModel { get; private set; }
	}
}
