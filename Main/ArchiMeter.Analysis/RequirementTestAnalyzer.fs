// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTestAnalyzer.fs" company="Roche.dk">
//   Copyright © Roche.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RequirementTestAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.Analysis
open ArchiMeter.Common
open Roslyn.Services
open Roslyn.Compilers.Common
open Roslyn.Compilers.CSharp
open System.IO
open System.Linq
open System

type RequirementTestAnalyzer(solutionProvider : IProvider<string, IProject>) =
    let _provider = solutionProvider
    let allSyntaxNodes(sn : SyntaxNode) = true
    let isTestMethod(syntax : MethodDeclarationSyntax) =
        syntax.AttributeLists 
        |> Seq.exists (fun l -> l.Attributes 
                                |> Seq.exists (fun a -> match a.Name with 
                                                        | :? SimpleNameSyntax as s -> [|"TestMethod"; "Test"; "TestCase"; "Fact"|] |> Seq.exists(fun t -> s.Identifier.ValueText = t)
                                                        | _ -> false))
    let getTests(p : string) = 
        _provider.GetAll(p)
        |> Seq.where (fun p -> p <> null)
        |> Seq.distinctBy (fun p -> p.FilePath)
        |> Seq.collect (fun p -> p.Documents)
        |> Seq.distinctBy (fun d -> d.FilePath)
        |> Seq.map (fun d -> d.GetSyntaxRoot() :?> SyntaxNode)
        |> Seq.where (fun n -> n <> null)
        |> Seq.collect (fun (n : SyntaxNode) -> n.DescendantNodes(allSyntaxNodes))
        |> Seq.where (fun (n : SyntaxNode) -> n.Kind = SyntaxKind.MethodDeclaration)
        |> Seq.cast<MethodDeclarationSyntax>
        |> Seq.where isTestMethod
        |> Seq.toArray
    let getAssertCount(node : MethodDeclarationSyntax) = 
        let descendants = node.DescendantNodes(allSyntaxNodes)
        let asserts = descendants
                        |> Seq.where (fun n -> n.Kind = SyntaxKind.MemberAccessExpression)
                        |> Seq.map (fun n -> n :?> MemberAccessExpressionSyntax)
                        |> Seq.map(fun n -> match n.Expression with | :? SimpleNameSyntax as x -> Some(x) | _ -> None)
                        |> Seq.where Option.isSome
                        |> Seq.where (fun n -> n.Value.Identifier.ValueText = "Assert" || n.Value.Identifier.ValueText = "ExceptionAssert")
                        |> Seq.length
        let mockCount = descendants
                        |> Seq.where (fun n -> n.Kind = SyntaxKind.MemberAccessExpression)
                        |> Seq.map (fun n -> n :?> MemberAccessExpressionSyntax)
                        |> Seq.where (fun n -> n.Name.Identifier.ValueText = "Verify" || n.Name.Identifier.ValueText = "VerifySet" || n.Name.Identifier.ValueText = "VerifyGet")
                        |> Seq.length
        let expectationCount = node.AttributeLists
                               |> Seq.where (fun l -> l.Attributes |> Seq.exists (fun a -> match a.Name with | :? SimpleNameSyntax as n -> n.Identifier.ValueText = "ExpectedException" | _ -> false))
                               |> Seq.length
        asserts + mockCount + expectationCount
    let getTestProperties(node : SyntaxNode) =
        match node with
        | n when n.Kind = SyntaxKind.Attribute && (n :?> AttributeSyntax).Name.ToFullString() = "TestProperty" -> Some(n :?> AttributeSyntax)
        | _ -> None
    let getRequirementsProperties(node : AttributeSyntax) =
        match node with
        | n when n.ArgumentList <> null && n.ArgumentList.Arguments |> Seq.exists (fun a -> a.Expression.ToFullString() = "TC.Requirement") -> n.ArgumentList.Arguments 
                                                                                                                                                |> Seq.where(fun a -> a.Expression.ToFullString() <> "TC.Requirement")
                                                                                                                                                |> Seq.map (fun a -> a.ToFullString().Trim('"'))
                                                                                                                                                |> Seq.collect (fun s -> s.Split(','))
                                                                                                                                                |> Seq.map (fun s -> s.Trim())
                                                                                                                                                |> Seq.map (fun s -> match s with | "" -> "0" | _ -> s)
        | _ -> Seq.empty
    let getRequirementsForTest(node : MethodDeclarationSyntax) =
        let children = node.ChildNodes()
        let reqAttribute = children
                            |> Seq.where (fun (n : SyntaxNode) -> n.Kind = SyntaxKind.AttributeList)
                            |> Seq.cast<AttributeListSyntax>
                            |> Seq.collect (fun (n : AttributeListSyntax) -> n.ChildNodes() |> Seq.map getTestProperties)
                            |> Utils.GetValues
                            |> Seq.map getRequirementsProperties
                            |> Seq.collect (fun r -> r)
                            |> Seq.map (fun s -> int s)
        let assertCount = getAssertCount node
        new TestData(reqAttribute, assertCount, node.Identifier.ValueText, node.ToFullString())
    interface IRequirementTestAnalyzer with
        member this.GetTestData(path : string) =
            let tests = getTests path
            let requirements = tests |> Seq.map getRequirementsForTest
            requirements
        member this.GetRequirementTests(path : string) =
            let testData = (this :> IRequirementTestAnalyzer).GetTestData path
            let allRequirements = testData |> Seq.collect (fun d -> d.RequirementIds) |> Seq.distinct
            allRequirements |> Seq.map (fun r -> new RequirementToTestReport(r, testData |> Seq.where(fun d -> d.RequirementIds |> Seq.exists (fun i -> i = r))))