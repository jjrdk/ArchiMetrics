namespace SolutionAnalyzer

type IModelVisitor =
    abstract Visit : seq<EA.Package> -> unit