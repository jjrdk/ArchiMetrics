namespace ArchiMeter.Analysis.Xaml
open ArchiMeter.Common.Xaml
open System.Xml.Linq
open System.Xml
open System.IO
open System

type XamlConverter() =
    static let _codeWriter = new XamlCodeWriter()
    let getNodeName(propertyName : string, currentXamlNode : XElement) = 
        match String.IsNullOrWhiteSpace propertyName with 
        | true -> currentXamlNode.Name.LocalName 
        | _ -> propertyName
    member this.ConvertSnippet(snippet : string) =
        let doc = XDocument.Parse snippet
        let node = new XamlNode(null, doc.Root)
        _codeWriter.CreateSyntax node
    member this.Convert(filepath : string) =
        let text = File.ReadAllText(filepath)
        this.ConvertSnippet(text)