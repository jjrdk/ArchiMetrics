// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockingOnWeakIdentityObjectRule.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LockingOnWeakIdentityObjectRule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Semantic
{
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common.CodeReview;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal class LockingOnWeakIdentityObjectRule : SemanticEvaluationBase
	{
		private static readonly string[] WeakIdentities =
		{
			"System.MarshalByRefObject", 
			"System.ExecutionEngineException", 
			"System.OutOfMemoryException", 
			"System.StackOverflowException", 
			"string", 
			"System.Reflection.MemberInfo", 
			"System.Reflection.ParameterInfo", 
			"System.Threading.Thread"
		};

		public override string ID
		{
			get
			{
				return "AMS0004";
			}
		}

		public override string Suggestion
		{
			get
			{
				return "Change lock object to strong identity object, ex. new object()";
			}
		}

		public override CodeQuality Quality
		{
			get
			{
				return CodeQuality.Broken;
			}
		}

		public override QualityAttribute QualityAttribute
		{
			get
			{
				return QualityAttribute.CodeQuality;
			}
		}

		public override ImpactLevel ImpactLevel
		{
			get
			{
				return ImpactLevel.Type;
			}
		}

		public override SyntaxKind EvaluatedKind
		{
			get
			{
				return SyntaxKind.LockStatement;
			}
		}

		public override string Title
		{
			get
			{
				return "No locking on Weak Identity Items";
			}
		}

		protected override Task<EvaluationResult> EvaluateImpl(SyntaxNode node, SemanticModel semanticModel, Solution solution)
		{
			var lockStatement = (LockStatementSyntax)node;
			var lockExpression = lockStatement.Expression as IdentifierNameSyntax;
			if (lockExpression != null)
			{
				var lockObjectSymbolInfo = semanticModel.GetSymbolInfo(lockExpression);
				var symbol = lockObjectSymbolInfo.Symbol as IFieldSymbol;
				if (symbol != null && IsWeakIdentity(symbol.Type))
				{
					return Task.FromResult(new EvaluationResult
						   {
							   Snippet = node.ToFullString()
						   });
				}
			}

			return Task.FromResult((EvaluationResult)null);
		}

		private bool IsWeakIdentity(ITypeSymbol typeSymbol)
		{
			return typeSymbol != null && (WeakIdentities.Contains(typeSymbol.ToString()) || IsWeakIdentity(typeSymbol.BaseType));
		}
	}
}