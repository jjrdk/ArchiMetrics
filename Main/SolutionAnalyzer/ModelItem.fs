namespace SolutionAnalyzer
open System
open System.Collections.Generic

[<AllowNullLiteral>]
[<Serializable>]
type ModelItem(name : string, linesOfCode : int, elementType : ElementType, properties : seq<IPropertyDefinition>, methods : seq<IMethodDefinition>) = 
    let _properties = new HashSet<IPropertyDefinition>((Seq.toArray properties))
    let _methods = new HashSet<IMethodDefinition>((Seq.toArray methods))
    member this.Name = name
    member this.LinesOfCode = linesOfCode
    member this.Type = elementType
    member this.Properties : seq<IPropertyDefinition> = _properties :> seq<IPropertyDefinition>
    member this.Methods : seq<IMethodDefinition> = _methods :> seq<IMethodDefinition>
    interface IEquatable<ModelItem> with
        member this.Equals(item : ModelItem) = 
            if obj.ReferenceEquals(item, null) then
                false
            else 
                if obj.ReferenceEquals(item, this) then
                    true
                else 
                    this.Type = item.Type 
                    && _properties.Count = Seq.sumBy(fun t -> 1) item.Properties
                    && _properties.SetEquals item.Properties
                    && _methods.Count = Seq.sumBy(fun t -> 1) item.Methods
                    && _methods.SetEquals item.Methods
    override this.GetHashCode() =
        hash this.Type * hash _properties * hash _methods
    override this.Equals(o : obj) = 
        match o with
        | :? ModelItem as y -> (this :> IEquatable<ModelItem>).Equals y
        | _ -> false