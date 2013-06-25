namespace ArchiMeter.Analysis
open ArchiMeter.Common
open System
open System.Collections.Generic

type DependencyChain(referenceChain : seq<EdgeItem>, root : EdgeItem, lastEdge : EdgeItem) = 
    let keys = fun (e : EdgeItem) -> e.Dependant
    let _rootDependant = root.Dependant
    let _lastDependency = lastEdge.Dependency
    let _referenceChain = Seq.concat [referenceChain; Seq.ofArray [|lastEdge|]] |> Seq.toArray
    let getName() = 
        let firstLink = _referenceChain |> Seq.map keys |> Seq.sort |> Seq.head
        let startIndex = _referenceChain |> Seq.findIndex (fun e -> e.Dependant = firstLink)
        let startSeq = _referenceChain |> Seq.skip startIndex
        let endSeq = _referenceChain |> Seq.take startIndex
        String.Join(" --> ", [|startSeq; endSeq|] |> Seq.concat)
    let _name = getName()
    let _hashCode = _name.GetHashCode()
    member this.ReferenceChain = _referenceChain |> Seq.ofArray
    member this.Root = root
    member this.LastEdge = lastEdge
    member this.ChainLength = Seq.length _referenceChain
    member this.IsCircular = _rootDependant = _lastDependency
    member this.IsContinuation(edge : EdgeItem) = _lastDependency = edge.Dependant && not (_referenceChain |> Seq.exists(fun (e : EdgeItem) -> e.Dependant = edge.Dependant))
    member this.Contains(edge : EdgeItem) = _referenceChain |> Seq.exists (fun e -> e.Dependant = edge.Dependant && e.Dependency = edge.Dependency)
    override this.ToString() = _name
    override this.GetHashCode() = _hashCode
    override this.Equals(o : obj) =
        match o with
        | null -> false
        | :? DependencyChain as d -> d.GetHashCode() = _hashCode
        | _ -> false
