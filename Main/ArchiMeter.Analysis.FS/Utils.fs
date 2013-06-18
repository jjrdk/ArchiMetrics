namespace ArchiMeter.Analysis

type internal Utils() =
    static member GetValue(item : option<'a>) =
        item.Value
    static member GetValues(source) =
        source
        |> Seq.where Option.isSome
        |> Seq.map (fun o -> o.Value)