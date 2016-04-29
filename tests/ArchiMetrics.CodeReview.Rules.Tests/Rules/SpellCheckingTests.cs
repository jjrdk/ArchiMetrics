// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellCheckingTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SpellCheckingTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.CodeReview.Rules.Tests.Rules
{
    using System.Linq;
    using System.Threading.Tasks;
    using Analysis.Common;
    using Analysis.Common.CodeReview;
    using ArchiMetrics.Analysis;
    using ArchiMetrics.CodeReview.Rules.Code;
    using ArchiMetrics.CodeReview.Rules.Trivia;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Xunit;

    public sealed class SpellCheckingTests
    {
        private SpellCheckingTests()
        {
        }

        public class GivenAMethodNameSpellingRule
        {
            private readonly MethodNameSpellingRule _rule;

            public GivenAMethodNameSpellingRule()
            {
                _rule = new MethodNameSpellingRule(new SpellChecker(new ExemptPatterns()));
            }

            [Theory]
            [InlineData("SomMethod")]
            [InlineData("CalclateValue")]
            [InlineData("GetValu")]
            public void FindMispelledMethodNames(string methodName)
            {
                var method = CSharpSyntaxTree.ParseText(string.Format(@"public void {0}() {{ }}", methodName));
                var result = _rule.Evaluate(method.GetRoot()
                    .ChildNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .First());

                Assert.NotNull(result);
            }
        }

        public class GivenAMultiLineCommentLanguageRule
        {
            private readonly MultiLineCommentLanguageRule _rule;

            public GivenAMultiLineCommentLanguageRule()
            {
                _rule = new MultiLineCommentLanguageRule(new SpellChecker(new ExemptPatterns()));
            }

            [Theory]
            [InlineData("ASP.NET MVC is a .NET acronym.")]
            [InlineData("Donde esta la cerveza?")]
            [InlineData("Dette er ikke en engelsk kommentar.")]
            public void FindNonEnglishMultiLineComments(string comment)
            {
                var method = CSharpSyntaxTree.ParseText(
                    string.Format(
@"public void SomeMethod() {{
/* {0} */
}}",
   comment));
                var root = method.GetRoot().DescendantNodes().OfType<BlockSyntax>().First();
                var nodes = root
                    .DescendantTrivia(descendIntoTrivia: true)
                    .Where(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia))
                    .AsArray();
                var result = _rule.Evaluate(nodes.First());

                Assert.NotNull(result);
            }

            [Theory]
            [InlineData(".NET has syntactic sugar the iterator pattern.")]
            [InlineData("This comment is in English.")]
            public void AcceptsEnglishMultiLineComments(string comment)
            {
                var method = CSharpSyntaxTree.ParseText(
                    string.Format(
@"public void SomeMethod() {{
/* {0} */
}}",
   comment));
                var root = method.GetRoot().DescendantNodes().OfType<BlockSyntax>().First();
                var nodes = root
                    .DescendantTrivia(descendIntoTrivia: true)
                    .Where(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia))
                    .AsArray();
                var result = _rule.Evaluate(nodes.First());

                Assert.Null(result);
            }

            [Theory]
            [InlineData("<summary>Returns a string.</summary>")]
            [InlineData("<returns>A string.</returns>")]
            public void AcceptsEnglishMultiLineXmlComments(string comment)
            {
                var method = CSharpSyntaxTree.ParseText(
                    string.Format(
                        @"public void SomeMethod() {{
/* {0} */
}}",
                        comment));
                var root = method.GetRoot().DescendantNodes().OfType<BlockSyntax>().First();
                var nodes = root
                    .DescendantTrivia(descendIntoTrivia: true)
                    .Where(t => t.IsKind(SyntaxKind.MultiLineCommentTrivia))
                    .AsArray();
                var result = _rule.Evaluate(nodes.First());

                Assert.Null(result);
            }
        }

        public class GivenASingleLineCommentLanguageRule
        {
            private readonly SingleLineCommentLanguageRule _rule;

            public GivenASingleLineCommentLanguageRule()
            {
                _rule = new SingleLineCommentLanguageRule(new SpellChecker(new ExemptPatterns()));
            }

            [Theory]
            [InlineData("Dette er ikke en engelsk kommentar.")]
            [InlineData("<returns>Noget tekst.</returns>")]
            public void FindNonEnglishSingleLineComments(string comment)
            {
                var method = CSharpSyntaxTree.ParseText(
                    string.Format(
@"public void SomeMethod() {{
//{0}
}}",
   comment));
                var root = method.GetRoot().DescendantNodes().OfType<BlockSyntax>().First();
                var nodes = root
                    .DescendantTrivia(descendIntoTrivia: true)
                    .Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    .AsArray();
                var result = _rule.Evaluate(nodes.First());

                Assert.NotNull(result);
            }
        }

        public class GivenASolutionInspectorWithCommentLanguageRules
        {
            private readonly NodeReviewer _reviewer;

            public GivenASolutionInspectorWithCommentLanguageRules()
            {
                var spellChecker = new SpellChecker(new ExemptPatterns());
                _reviewer = new NodeReviewer(new IEvaluation[] { new SingleLineCommentLanguageRule(spellChecker), new MultiLineCommentLanguageRule(spellChecker) }, Enumerable.Empty<ISymbolEvaluation>());
            }

            [Theory]
            [InlineData("//Dette er ikke en engelsk kommentar.")]
            [InlineData("// <summary>Dette er ikke en engelsk kommentar.</summary>")]
            [InlineData("/* Dette er ikke en engelsk kommentar. */")]
            public async Task WhenInspectingCommentsThenDetectsSuspiciousLanguage(string comment)
            {
                var method = CSharpSyntaxTree.ParseText(
                    string.Format(
@"public void SomeMethod() {{
{0}
}}",
   comment));
                var root = method.GetRoot();

                var task = await _reviewer.Inspect(string.Empty, string.Empty, root, null, null);

                Assert.NotEmpty(task);
            }
        }
    }
}
