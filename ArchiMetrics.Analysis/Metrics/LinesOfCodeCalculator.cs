// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinesOfCodeCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LinesOfCodeCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp.Syntax;

	internal sealed class LinesOfCodeCalculator
	{
		public int Calculate(SyntaxNode node)
		{
			var innerCalculator = new InnerLinesOfCodeCalculator();
			return innerCalculator.Calculate(node);
		}

		private class InnerLinesOfCodeCalculator : SyntaxWalker
		{
			private int _counter;

			public InnerLinesOfCodeCalculator()
				: base(SyntaxWalkerDepth.Node)
			{
			}

			public int Calculate(SyntaxNode node)
			{
				if (node != null)
				{
					Visit(node);
				}

				return _counter;
			}

			/// <summary>
			/// Called when the walker visits a node.  This method may be overridden if subclasses want
			///             to handle the node.  Overrides should call back into this base method if they want the
			///             children of this node to be visited.
			/// </summary>
			/// <param name="node">The current node that the walker is visiting.</param>
			public override void Visit(SyntaxNode node)
			{
				var statement = node as StatementSyntax;
				if (statement != null)
				{
					_counter++;
				}
				else
				{
					var accessor = node as AccessorDeclarationSyntax;
					if (accessor != null && accessor.Body == null)
					{
						_counter++;
					}
					else
					{
						var initializer = node as InitializerExpressionSyntax;
						if (initializer != null)
						{
							_counter += initializer.Expressions.Count;
						}
						else
						{
							var constructor = node as ConstructorDeclarationSyntax;
							if (constructor != null)
							{
								_counter++;
							}
							else
							{
								var usingDirective = node as UsingDirectiveSyntax;
								if (usingDirective != null)
								{
									_counter++;
								}
							}
						}
					}
				}
				base.Visit(node);
			}
		}
	}
}