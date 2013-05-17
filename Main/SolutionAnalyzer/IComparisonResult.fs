namespace SolutionAnalyzer

type IComparisonResult = 
    abstract Name : string
    abstract Type : ElementType
    abstract Implementation : ImplementationType
    abstract Properties : seq<IPropertyDefinition>
    abstract Methods : seq<IMethodDefinition>