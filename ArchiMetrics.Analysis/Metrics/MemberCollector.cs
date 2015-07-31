// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberCollector.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberCollector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace ArchiMetrics.Analysis.Metrics
{
    using System.Collections.Generic;
    using System.Linq;
    using ArchiMetrics.Common.Metrics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class MemberCollector : CSharpSyntaxWalker
    {
        private readonly List<SyntaxNode> _members;

        public MemberCollector()
            : base(SyntaxWalkerDepth.Node)
        {
            _members = new List<SyntaxNode>();
        }

        public IEnumerable<SyntaxNode> GetMembers(TypeDeclarationSyntaxInfo type)
        {
            Visit(type.Syntax);
            return _members.ToList();
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            base.VisitConstructorDeclaration(node);
            _members.Add(node);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            base.VisitDestructorDeclaration(node);
            _members.Add(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            base.VisitEventDeclaration(node);
            if (node.AccessorList != null)
            {
                foreach (var accessor in node.AccessorList.Accessors)
                {
                    _members.Add(accessor);
                }
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
            _members.Add(node);
        }

        /// <summary>
        /// Called when the visitor visits a ArrowExpressionClauseSyntax node.
        /// </summary>
        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            var accessor = SyntaxFactory.ReturnStatement(node.Expression);
            
            _members.Add(accessor);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);
            if (node.AccessorList != null)
            {
                foreach (var accessor in node.AccessorList.Accessors)
                {
                    _members.Add(accessor);
                }
            }
        }
    }
}
