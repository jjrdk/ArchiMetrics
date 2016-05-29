// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operators.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the Operators type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using System.Collections.Generic;
	using Microsoft.CodeAnalysis.CSharp;

	internal sealed class Operators
	{
		public static readonly IEnumerable<SyntaxKind> All = new[]
			                                                     {
				                                                     SyntaxKind.DotToken, 
																	 SyntaxKind.EqualsToken, 
				                                                     SyntaxKind.SemicolonToken, 
																	 SyntaxKind.PlusPlusToken, 
				                                                     SyntaxKind.PlusToken, 
																	 SyntaxKind.PlusEqualsToken, 
				                                                     SyntaxKind.MinusMinusToken, 
																	 SyntaxKind.MinusToken, 
				                                                     SyntaxKind.MinusEqualsToken, 
																	 SyntaxKind.AsteriskToken, 
				                                                     SyntaxKind.AsteriskEqualsToken, 
																	 SyntaxKind.SlashToken, 
				                                                     SyntaxKind.SlashEqualsToken, 
																	 SyntaxKind.PercentToken, 
				                                                     SyntaxKind.PercentEqualsToken, 
																	 SyntaxKind.AmpersandToken, 
				                                                     SyntaxKind.BarToken, 
																	 SyntaxKind.CaretToken, 
				                                                     SyntaxKind.TildeToken, 
																	 SyntaxKind.ExclamationToken, 
				                                                     SyntaxKind.ExclamationEqualsToken, 
																	 SyntaxKind.GreaterThanToken, 
				                                                     SyntaxKind.GreaterThanEqualsToken, 
																	 SyntaxKind.LessThanToken, 
				                                                     SyntaxKind.LessThanEqualsToken
			                                                     };
	}
}
