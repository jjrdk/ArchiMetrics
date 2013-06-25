namespace ArchiMeter.Analysis
open System

type RequirementToTestReport(requirementId : int, coveringTests : seq<TestData>) =
    member this.RequirementId = requirementId
    member this.CoveringTestNames = coveringTests |> Seq.map (fun d -> d.TestName) |> Seq.toArray
    member this.CoveringTests = coveringTests |> Seq.map(fun d -> d.TestCode) |> Seq.toArray
    member this.AssertsPerTest = coveringTests |> Seq.groupBy(fun d -> d.AssertCount) |> Seq.map (fun (k, g) -> (k, Seq.length g)) |> Seq.toArray