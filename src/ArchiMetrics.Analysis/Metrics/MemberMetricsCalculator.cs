// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberMetricsCalculator.cs" company="Reimers.dk">
//   Copyright © Matthias Friedrich, Reimers.dk 2014
//   This source is subject to the MIT License.
//   Please see https://opensource.org/licenses/MIT for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MemberMetricsCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.Analysis.Metrics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Common.Metrics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class MemberMetricsCalculator : SemanticModelMetricsCalculator
    {
        private readonly CyclomaticComplexityCounter _counter = new CyclomaticComplexityCounter();
        private readonly LinesOfCodeCalculator _locCalculator = new LinesOfCodeCalculator();
        private readonly MemberNameResolver _nameResolver;
        private readonly Solution _solution;
        private readonly string _rootFolder;
        private readonly IAsyncFactory<ISymbol, IMemberDocumentation> _documentationFactory;

        public MemberMetricsCalculator(SemanticModel semanticModel, Solution solution, string rootFolder, IAsyncFactory<ISymbol, IMemberDocumentation> documentationFactory)
            : base(semanticModel)
        {
            _solution = solution;
            _rootFolder = rootFolder;
            _documentationFactory = documentationFactory;
            _nameResolver = new MemberNameResolver(Model);
        }

        public async Task<IEnumerable<IMemberMetric>> Calculate(TypeDeclarationSyntaxInfo typeNode)
        {
            var walker = new MemberCollector();
            var members = walker.GetMembers(typeNode).AsArray();
            if ((typeNode.Syntax is ClassDeclarationSyntax
                || typeNode.Syntax is StructDeclarationSyntax)
                && members.All(m => m.Kind() != SyntaxKind.ConstructorDeclaration))
            {
                var defaultConstructor = SyntaxFactory.ConstructorDeclaration(typeNode.Name)
                                               .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                               .WithBody(SyntaxFactory.Block());
                members = members.Concat(new[] { defaultConstructor }).AsArray();
            }

            var metrics = await CalculateMemberMetrics(members).ConfigureAwait(false);
            return metrics.AsArray();
        }

        public IMemberMetric CalculateSlim(MethodDeclarationSyntax methodDeclaration)
        {
            return CalculateMemberMetricSlim(methodDeclaration);
        }

        private static double CalculateMaintainablityIndex(double cyclomaticComplexity, double linesOfCode, IHalsteadMetrics halsteadMetrics)
        {
            if (linesOfCode.Equals(0.0) || halsteadMetrics.NumberOfOperands.Equals(0) || halsteadMetrics.NumberOfOperators.Equals(0))
            {
                return 100.0;
            }

            var num = Math.Log(halsteadMetrics.GetVolume());
            var mi = ((171 - (5.2 * num) - (0.23 * cyclomaticComplexity) - (16.2 * Math.Log(linesOfCode))) * 100) / 171;

            return Math.Max(0.0, mi);
        }

        private int CalculateLinesOfCode(SyntaxNode node)
        {
            return _locCalculator.Calculate(node);
        }

        private int CalculateCyclomaticComplexity(SyntaxNode node)
        {
            return _counter.Calculate(node, Model);
        }

        private IEnumerable<ITypeCoupling> CalculateClassCoupling(SyntaxNode node)
        {
            var provider = new MemberClassCouplingAnalyzer(Model);
            return provider.Calculate(node);
        }

        private async Task<IEnumerable<IMemberMetric>> CalculateMemberMetrics(IEnumerable<SyntaxNode> nodes)
        {
            var tasks = nodes.Select(CalculateMemberMetric);

            var metrics = await Task.WhenAll(tasks).ConfigureAwait(false);
            return from metric in metrics
                   where metric != null
                   select metric;
        }

        private async Task<IMemberMetric> CalculateMemberMetric(SyntaxNode syntaxNode)
        {
            var analyzer = new HalsteadAnalyzer();
            var halsteadMetrics = analyzer.Calculate(syntaxNode);
            var memberName = _nameResolver.TryResolveMemberSignatureString(syntaxNode);
            var source = CalculateClassCoupling(syntaxNode);
            var complexity = CalculateCyclomaticComplexity(syntaxNode);
            var linesOfCode = CalculateLinesOfCode(syntaxNode);
            var numberOfParameters = CalculateNumberOfParameters(syntaxNode);
            var numberOfLocalVariables = CalculateNumberOfLocalVariables(syntaxNode);
            var maintainabilityIndex = CalculateMaintainablityIndex(complexity, linesOfCode, halsteadMetrics);
            var afferentCoupling = await CalculateAfferentCoupling(syntaxNode).ConfigureAwait(false);
            var location = syntaxNode.GetLocation();
            var lineNumber = location.GetLineSpan().StartLinePosition.Line;
            var filePath = location.SourceTree == null ? string.Empty : location.SourceTree.FilePath;
            filePath = filePath.GetPathRelativeTo(_rootFolder);
            var accessModifier = GetAccessModifier(syntaxNode);
            IMemberDocumentation documentation = null;

            if (syntaxNode.SyntaxTree == Model.SyntaxTree)
            {
                var symbol = Model.GetDeclaredSymbol(syntaxNode);
                documentation = await _documentationFactory.Create(symbol, CancellationToken.None);
            }

            return new MemberMetric(filePath,
                accessModifier,
                halsteadMetrics,
                lineNumber,
                linesOfCode,
                maintainabilityIndex,
                complexity,
                memberName,
                source.AsArray(),
                numberOfParameters,
                numberOfLocalVariables,
                afferentCoupling,
                documentation);
        }

        private IMemberMetric CalculateMemberMetricSlim(SyntaxNode syntaxNode)
        {
            var analyzer = new HalsteadAnalyzer();
            var halsteadMetrics = analyzer.Calculate(syntaxNode);
            var memberName = _nameResolver.TryResolveMemberSignatureString(syntaxNode);
            var source = Enumerable.Empty<ITypeCoupling>();
            var complexity = CalculateCyclomaticComplexity(syntaxNode);
            var linesOfCode = CalculateLinesOfCode(syntaxNode);
            const int NumberOfParameters = 0;
            const int NumberOfLocalVariables = 0;
            var maintainabilityIndex = CalculateMaintainablityIndex(complexity, linesOfCode, halsteadMetrics);
            const int AfferentCoupling = 0;
            const int LineNumber = 0;
            var filePath = string.Empty;
            var accessModifier = GetAccessModifier(syntaxNode);
            return new MemberMetric(filePath,
                accessModifier,
                halsteadMetrics,
                LineNumber,
                linesOfCode,
                maintainabilityIndex,
                complexity,
                memberName,
                source.AsArray(),
                NumberOfParameters,
                NumberOfLocalVariables,
                AfferentCoupling,
                null);
        }

        private AccessModifierKind GetAccessModifier(SyntaxNode node)
        {
            var method = node as BaseMethodDeclarationSyntax;
            if (method != null)
            {
                return GetAccessModifier(method.Modifiers);
            }

            var property = node as BasePropertyDeclarationSyntax;
            if (property != null)
            {
                return GetAccessModifier(property.Modifiers);
            }

            return AccessModifierKind.Private;
        }

        private AccessModifierKind GetAccessModifier(SyntaxTokenList tokenList)
        {
            if (tokenList.Any(SyntaxKind.PublicKeyword))
            {
                return AccessModifierKind.Public;
            }

            if (tokenList.Any(SyntaxKind.ProtectedKeyword))
            {
                var isInternal = tokenList.Any(SyntaxKind.InternalKeyword);
                return isInternal ? AccessModifierKind.Internal | AccessModifierKind.Protected : AccessModifierKind.Protected;
            }

            return AccessModifierKind.Private;
        }

        private async Task<int> CalculateAfferentCoupling(SyntaxNode node)
        {
            try
            {
                if (_solution == null)
                {
                    return 0;
                }

                if (node.SyntaxTree != Model.SyntaxTree)
                {
                    return 0;
                }

                var symbol = Model.GetDeclaredSymbol(node);
                var referenceTasks = symbol == null
                ? Task.FromResult(0)
                : _solution.FindReferences(symbol)
                    .ContinueWith(t => t.Exception != null ? 0 : t.Result.Locations.Count());

                return await referenceTasks.ConfigureAwait(false);
            }
            catch
            {
                // Some constructors are not present in syntax tree because they have been created for metrics calculation.
                return 0;
            }
        }

        private int CalculateNumberOfLocalVariables(SyntaxNode node)
        {
            var analyzer = new MethodLocalVariablesAnalyzer();
            return analyzer.Calculate(node);
        }

        private int CalculateNumberOfParameters(SyntaxNode node)
        {
            var member = node as BaseMethodDeclarationSyntax;
            if (member != null)
            {
                return member.ParameterList.Parameters.Count;
            }

            var accessor = node as AccessorDeclarationSyntax;
            return accessor != null && accessor.IsKind(SyntaxKind.SetAccessorDeclaration) ? 1 : 0;
        }
    }
}
