namespace SolutionAnalyzer
open System

type IMethodDefinition = 
    inherit IMemberDefinition
    inherit IComparable
    abstract Implementation : ImplementationType
    abstract Parameters : seq<string*string*ImplementationType>