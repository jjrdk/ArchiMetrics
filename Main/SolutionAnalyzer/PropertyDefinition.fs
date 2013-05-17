namespace SolutionAnalyzer
open System
open System.Collections.Generic

[<Serializable>]
type PropertyDefinition(name : string, returnType : string, implementation : ImplementationType) =
    interface IMemberDefinition with
        member this.Name = name
        member this.ReturnType = returnType
    interface IPropertyDefinition with
        member this.Implementation = implementation    
    interface IComparable with
        member this.CompareTo(o : obj) =
            match o with
            | :? IMemberDefinition as o1 -> (this :> IMemberDefinition).Name.CompareTo(o1.Name)
            | _ -> -1
    interface IEquatable<PropertyDefinition> with
        member this.Equals(item : PropertyDefinition) = 
            if obj.ReferenceEquals(item, null) then
                false
            else 
                if obj.ReferenceEquals(item, this) then
                    true
                else 
                    (this :> IPropertyDefinition).Name = (item :> IPropertyDefinition).Name && (this :> IPropertyDefinition).ReturnType = (item :> IPropertyDefinition).ReturnType
    override this.GetHashCode() =
        hash (this :> IMemberDefinition).Name * hash (this :> IPropertyDefinition).ReturnType
    override this.Equals(o : obj) = 
        match o with
        | :? PropertyDefinition as y -> (this :> IEquatable<PropertyDefinition>).Equals y
        | _ -> false