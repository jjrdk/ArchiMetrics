namespace SolutionAnalyzer
open System
open System.Linq
open System.Threading.Tasks
open Roslyn.Compilers.CSharp

type internal InnerModelReader() =
    inherit ModelVisitor()
    let mutable _modelItems : ModelItem list = []
    let getProperty(property : EA.Attribute) : IPropertyDefinition =
        new PropertyDefinition(
                                (match property.Name with | null -> String.Empty | _ -> property.Name),
                                (match property.Type with | null -> String.Empty | _ -> property.Type),
                                ImplementationType.OnlyInModel) :> IPropertyDefinition
    let createParameters(parameters : seq<EA.Parameter>) = 
         parameters |> Seq.map (fun p -> ((match p.Type with | null -> String.Empty | _ -> p.Type), (match p.Name with | null -> String.Empty | _ -> p.Name), ImplementationType.Unknown)) |> Seq.toArray |> Seq.ofArray
    let getMethod(m : EA.Method) : IMethodDefinition =
        new MethodDefinition(
                                (match m.Name with | null -> String.Empty | _ -> m.Name),
                                (match m.ReturnType with | null -> String.Empty | _ -> m.ReturnType),
                                ImplementationType.OnlyInModel,
                                (match m.Parameters with 
                                | null -> Seq.empty<string*string*ImplementationType> 
                                | _ -> createParameters(m.Parameters.OfType<EA.Parameter>()))) :> IMethodDefinition
    let createModelItem(element : EA.Element, elementType : ElementType) =
        let properties = match element.Attributes with | null -> Seq.empty<IPropertyDefinition> | seq -> seq.OfType<EA.Attribute>() |> Seq.map getProperty |> Seq.toArray |> Seq.ofArray
        let methods = match element.Methods with | null -> Seq.empty<IMethodDefinition> | seq -> seq.OfType<EA.Method>() |> Seq.map getMethod |> Seq.toArray |> Seq.ofArray
        new ModelItem(element.Name, 0, elementType, properties, methods)
    let addModelItem(item : ModelItem) =
        _modelItems <- item :: _modelItems            
    override this.VisitElement(element : EA.Element) =
        if not (String.IsNullOrWhiteSpace(element.Name)) then
            match element.Type with
            | "Class" -> addModelItem(createModelItem(element, ElementType.Class))
            | "Interface" -> addModelItem(createModelItem(element, ElementType.Interface))
            | _ -> ()
        base.VisitElement element
    member this.GetModel() : seq<ModelItem> =
        Seq.distinct _modelItems
