namespace ArchiMeter.Analysis
open System
open ArchiMeter.Common

type PathFilter(filter : Func<ProjectDefinition, bool>) =
    member this.Filter(path : ProjectDefinition) = filter.Invoke path
