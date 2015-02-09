// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinesOfCodeCalculator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
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
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class LinesOfCodeCalculator
    {
        public int Calculate(SyntaxNode node, CodeMetricsOptions options)
        {
            var innerCalculator = new InnerLinesOfCodeCalculator(options);
            return innerCalculator.Calculate(node);
        }

        private class InnerLinesOfCodeCalculator : CSharpSyntaxWalker
        {
            private readonly CodeMetricsOptions _options;
            private int _counter;

            public InnerLinesOfCodeCalculator(CodeMetricsOptions options = null)
                : base(SyntaxWalkerDepth.Node)
            {
                _options = options;
            }

            public int Calculate(SyntaxNode node)
            {

                if (node != null)
                {
                    if (_options != null && _options.PreciseLinesOfCodeCalculation)
                        _counter = node.SyntaxTree.GetRoot().GetText().Lines.Count;
                    else
                        Visit(node);
                }

                return _counter;
            }

            public override void VisitCheckedStatement(CheckedStatementSyntax node)
            {
                base.VisitCheckedStatement(node);
                _counter++;
            }

            public override void VisitDoStatement(DoStatementSyntax node)
            {
                base.VisitDoStatement(node);
                _counter++;
            }

            public override void VisitEmptyStatement(EmptyStatementSyntax node)
            {
                base.VisitEmptyStatement(node);
                _counter++;
            }

            public override void VisitExpressionStatement(ExpressionStatementSyntax node)
            {
                base.VisitExpressionStatement(node);
                _counter++;
            }

            /// <summary>
            /// Called when the visitor visits a AccessorDeclarationSyntax node.
            /// </summary>
            public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
            {
                if (node.Body == null)
                {
                    _counter++;
                }

                base.VisitAccessorDeclaration(node);
            }

            public override void VisitFixedStatement(FixedStatementSyntax node)
            {
                base.VisitFixedStatement(node);
                _counter++;
            }

            public override void VisitForEachStatement(ForEachStatementSyntax node)
            {
                base.VisitForEachStatement(node);
                _counter++;
            }

            public override void VisitForStatement(ForStatementSyntax node)
            {
                base.VisitForStatement(node);
                _counter++;
            }

            public override void VisitGlobalStatement(GlobalStatementSyntax node)
            {
                base.VisitGlobalStatement(node);
                _counter++;
            }

            public override void VisitGotoStatement(GotoStatementSyntax node)
            {
                base.VisitGotoStatement(node);
                _counter++;
            }

            public override void VisitIfStatement(IfStatementSyntax node)
            {
                base.VisitIfStatement(node);
                _counter++;
            }

            public override void VisitInitializerExpression(InitializerExpressionSyntax node)
            {
                base.VisitInitializerExpression(node);
                _counter += node.Expressions.Count;
            }

            public override void VisitLabeledStatement(LabeledStatementSyntax node)
            {
                base.VisitLabeledStatement(node);
                _counter++;
            }

            public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
            {
                base.VisitLocalDeclarationStatement(node);
                if (!node.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    _counter++;
                }
            }

            public override void VisitLockStatement(LockStatementSyntax node)
            {
                base.VisitLockStatement(node);
                _counter++;
            }

            public override void VisitReturnStatement(ReturnStatementSyntax node)
            {
                base.VisitReturnStatement(node);
                if (node.Expression != null)
                {
                    _counter++;
                }
            }

            public override void VisitSwitchStatement(SwitchStatementSyntax node)
            {
                base.VisitSwitchStatement(node);
                _counter++;
            }

            public override void VisitThrowStatement(ThrowStatementSyntax node)
            {
                base.VisitThrowStatement(node);
                _counter++;
            }

            public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
            {
                base.VisitUnsafeStatement(node);
                _counter++;
            }

            public override void VisitUsingDirective(UsingDirectiveSyntax node)
            {
                base.VisitUsingDirective(node);
                _counter++;
            }

            public override void VisitUsingStatement(UsingStatementSyntax node)
            {
                base.VisitUsingStatement(node);
                _counter++;
            }

            /// <summary>
            /// Called when the visitor visits a ConstructorDeclarationSyntax node.
            /// </summary>
            public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                base.VisitConstructorDeclaration(node);
                _counter++;
            }

            public override void VisitWhileStatement(WhileStatementSyntax node)
            {
                base.VisitWhileStatement(node);
                _counter++;
            }

            public override void VisitYieldStatement(YieldStatementSyntax node)
            {
                base.VisitYieldStatement(node);
                _counter++;
            }
        }
    }
}