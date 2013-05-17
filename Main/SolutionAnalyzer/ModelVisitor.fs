namespace SolutionAnalyzer
open System
open System.Linq

type ModelVisitor() =
    let rec getPackagesRecursive(root : EA.Package) =
        seq { for package in root.Packages.OfType<EA.Package>() do
                yield package
                yield! getPackagesRecursive package }
    let visitConnector(connector : EA.Connector) = 
        ()
    abstract member VisitElement : element : EA.Element -> unit
    abstract member VisitPackage : package : EA.Package -> unit
    default this.VisitElement(element : EA.Element) =
        for connector in element.Connectors.OfType<EA.Connector>() do
            visitConnector connector
    default this.VisitPackage(package : EA.Package) =
        for element in package.Elements.OfType<EA.Element>() do
            this.VisitElement element
    interface IModelVisitor with
        member this.Visit(modelRoot : seq<EA.Package>) =
            for package in modelRoot |> Seq.collect getPackagesRecursive do
                this.VisitPackage package
            for element in modelRoot |> Seq.collect (fun p -> p.Elements.OfType<EA.Element>()) do
                this.VisitElement element