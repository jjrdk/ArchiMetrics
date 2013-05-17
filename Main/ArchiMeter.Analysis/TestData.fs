namespace ArchiMeter.Analysis

type TestData(requirementIds : seq<int>, assertCount : int, testName : string, testCode : string) =
    member this.RequirementIds = requirementIds
    member this.AssertCount = assertCount
    member this.TestCode = testCode
    member this.TestName = testName
