namespace SolutionAnalyzer
open System.Collections.Generic

type InnerModelNameComparer() =
    interface IEqualityComparer<ModelItem> with
        member this.Equals(x : ModelItem, y : ModelItem) =
            x.Name = y.Name
        member this.GetHashCode(obj : ModelItem) =
            obj.Name.GetHashCode()
