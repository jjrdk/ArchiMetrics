namespace SolutionAnalyzer
open System
open System.Collections.Generic

[<Serializable>]
type MethodDefinition(name : string, returnType : string, implementation : ImplementationType, parameters : seq<string*string*ImplementationType>) =
    interface IMemberDefinition with
        member this.Name = name
        member this.ReturnType = returnType
    interface IMethodDefinition with
        member this.Implementation = implementation
        member this.Parameters = parameters
    interface IComparable with
        member this.CompareTo(o : obj) =
            match o with
            | :? IMemberDefinition as o1 -> (this :> IMemberDefinition).Name.CompareTo(o1.Name)
            | _ -> -1
    interface IEquatable<MethodDefinition> with
        member this.Equals(item : MethodDefinition) = 
            if obj.ReferenceEquals(item, null) then
                false
            else 
                if obj.ReferenceEquals(item, this) then
                    true
                else 
                    (this :> IMethodDefinition).Name = (item :> IMethodDefinition).Name && (this :> IMethodDefinition).ReturnType = (item :> IMethodDefinition).ReturnType
    override this.GetHashCode() =
        hash (this :> IMemberDefinition).Name * hash (this :> IMethodDefinition).ReturnType
    override this.Equals(o : obj) = 
        match o with
        | :? MethodDefinition as y -> (this :> IEquatable<MethodDefinition>).Equals y
        | _ -> false