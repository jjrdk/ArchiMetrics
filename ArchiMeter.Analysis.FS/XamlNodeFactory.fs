namespace ArchiMeter.Analysis.Xaml
open System.Xml.Linq
open System.Xml
open System.Collections.Generic
open System
open System.Text.RegularExpressions
open ArchiMeter.Analysis
//
//type internal XamlNodeFactory() =
//    static let clrNamespace = "clr-namespace:"
//    let attributeParser = new AttributeParser()
//    let distinctString(a,b) = a + ":" + b
//    let getNamespaceName(a,b,c) = a
//    let getBaseClassName(a,b,c) = b
//    let getClassName(a,b,c) = c
//    let readNames(parent : XElement, attribute : XAttribute option) =
//        let baseClassName = parent.Name.LocalName
//        match Option.isNone attribute with
//        | true -> (String.Empty, String.Empty, baseClassName)
//        | _ -> 
//                let components = attribute.Value.Value.Split('.') |> Seq.toArray
//                let ns = String.Join(".", components |> Seq.take (components.Length - 1))
//                let className = components |> Seq.last
//                (ns, baseClassName, className)
//    let getNames (node : XElement) =
//        node.Attributes() |> Seq.tryFind (fun a -> a.Name.LocalName = "Class") |> (fun a -> readNames(node, a))
//    let getVariableName(node : XElement) =
//        node.Attributes() |> Seq.find (fun a -> a.Name.LocalName = "Name") |> (fun a -> a.Value)
//    let createChild(parent : XamlNode) (node : XNode) (nodeCreation : XamlNode*XElement -> XamlNode) = 
//        match node.NodeType with
//        | XmlNodeType.Element when not ((node :?> XElement).Name.LocalName.Contains(".")) -> Some(nodeCreation (parent, node :?> XElement))
//        | XmlNodeType.Text when (node :?> XText).NextNode <> null -> Some(nodeCreation (parent, (node :?> XText).NextNode :?> XElement))
//        | _ -> Option.None
//    let parseChildren(parent : XElement) (nodeCreation : XamlNode * XElement -> XamlNode) (parentNode : XamlNode) = 
//        parent.Nodes()
//        |> Seq.map (fun n -> createChild parentNode n nodeCreation)
//        |> Utils.GetValues
//    let getUsings(element : XElement) =
//        seq {
//            yield ("System", String.Empty)
//            yield ("System.Windows.Controls", String.Empty)
//            yield! element.DescendantsAndSelf()
//                           |> Seq.collect (fun e -> e.Attributes())
//                           |> Seq.where (fun a -> a.Value.StartsWith(clrNamespace))
//                           |> Seq.collect (fun a -> let cleanValue = a.Value.Replace(clrNamespace, String.Empty)
//                                                    [|(cleanValue, String.Empty);(cleanValue, a.Name.LocalName)|])
//                           |> Seq.distinctBy(distinctString)
//        }
//    let rec createNode(parent : XamlNode option, element : XElement, propertyCreation : seq<XElement> -> XamlNode -> seq<XamlPropertyNode>) =
//        let names = getNames element
//        let namespaceName = names |> getNamespaceName
//        let baseClassName = names |> getBaseClassName
//        let className = names |> getClassName
//        let nodeCreation = fun (p : XamlNode, e : XElement) -> createNode(Some(p), e, propertyCreation)
//        let children = parseChildren element nodeCreation
//        let attributes = attributeParser.Get element |> Seq.toArray
//        let properties = propertyCreation (element.Elements() |> Seq.where (fun e -> e.Name.LocalName.Contains(".")))
//        let usings = if Option.isSome parent then Seq.empty else getUsings element
//        new XamlNode(parent, usings, namespaceName, className, baseClassName, children, attributes, properties)
//    let rec createProperty(property : XElement) (propertyCreation : seq<XElement> -> XamlNode -> seq<XamlPropertyNode>) (owner : XamlNode) =
//        let nameTokens = property.Name.LocalName.Split('.')
//        let declarer = nameTokens |> Seq.head
//        let propertyName = match owner.ClassName = declarer || owner.BaseClassName = declarer with | true -> nameTokens |> Seq.last | _ -> property.Name.LocalName
//        let isDependency = owner.ClassName = declarer || owner.BaseClassName = declarer
//        let values = property.Elements() |> Seq.map (fun valueElement -> lazy(createNode(Some(owner), valueElement, fun e -> propertyCreation e) :> obj))
//        values |> Seq.map (fun value -> new XamlPropertyNode(propertyName, isDependency, value))
//    let rec createProperties(properties : seq<XElement>) (owner : XamlNode) =
//        properties |> Seq.collect (fun e -> createProperty e createProperties owner)
//    member this.Create(parent : XamlNode option, element : XElement) =
//        createNode(parent, element, fun e -> createProperties e)