namespace SolutionAnalyzer
open System

type IPropertyDefinition = 
    inherit IMemberDefinition
    inherit IComparable
    abstract Implementation : ImplementationType