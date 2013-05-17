namespace ArchiMeter.Analysis

type IRequirementTestAnalyzer =
    abstract GetTestData : path : string -> seq<TestData>
    abstract GetRequirementTests : path : string -> seq<RequirementToTestReport>
