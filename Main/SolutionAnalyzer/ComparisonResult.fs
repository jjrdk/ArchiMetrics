namespace SolutionAnalyzer
open System

type ComparisonResult(name : string, linesOfCode : int, elementType : ElementType, implementation : ImplementationType, propertyDefinitions : seq<IPropertyDefinition>, methodDefinitions : seq<IMethodDefinition>) = 
    interface IComparisonResult with
        member this.Name = name
        member this.Type = elementType
        member this.Implementation = implementation
        member this.Properties = propertyDefinitions
        member this.Methods = methodDefinitions
    member this.LinesOfCode = linesOfCode
    override this.ToString() =
        let icr = this :> IComparisonResult
        String.Format("{0} {1}", icr.Type, icr.Name)