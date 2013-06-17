namespace ArchiMeter.Analysis
open System.Threading.Tasks
open ArchiMeter.Common

type DependencyAnalyzer() = 
    let flatten = fun r -> r
    let rec getDependencyChain(chain : DependencyChain, source : seq<EdgeItem>) = 
        match chain.IsCircular with
        | true -> [|chain|] |> Seq.ofArray
        | _ -> source 
               |> Seq.where chain.IsContinuation
               |> Seq.collect (fun i -> getDependencyChain(new DependencyChain(chain.ReferenceChain, chain.Root, i), source))
    member this.GetCircularReferences(items : seq<EdgeItem>) =
        Async.StartAsTask(
            async {
                return items 
                |> Seq.map (fun e -> async { return getDependencyChain(new DependencyChain(Seq.empty, e, e), items) } )
                |> Async.Parallel
                |> Async.RunSynchronously
                |> Seq.collect flatten
                |> Seq.where (fun (c : DependencyChain) -> c.IsCircular) 
                |> Seq.distinct
        })