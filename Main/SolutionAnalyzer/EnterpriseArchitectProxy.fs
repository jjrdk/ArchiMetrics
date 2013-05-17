namespace SolutionAnalyzer

open System.Linq;

type EnterpriseArchitectProxy(projectFile : string) =
    let getRepository() =
        let ea = new EA.RepositoryClass()
        ea.OpenFile(projectFile) |> ignore
        ea
    let _logicalModels : seq<EA.Package> = getRepository().Models.OfType<EA.Package>().ToArray() |> Seq.ofArray
    member this.LogicalModels = _logicalModels
