namespace ArchiMeter.Analysis.Xaml
open System.Xml.Linq
open System.Text.RegularExpressions
open ArchiMeter.Analysis
open ArchiMeter.Common.Xaml

type internal AttributeParser() =
    static let TrueRegex = new Regex("^True$", RegexOptions.Compiled)
    static let FalseRegex = new Regex("^False$", RegexOptions.Compiled)
    let addAttribute(attribute : XAttribute, value : string, extraCode : string) =
        let cleanValue = FalseRegex.Replace(TrueRegex.Replace(value, "true"), "false")
        Some(new XamlNodeAttribute(attribute.Name.LocalName, value, extraCode))
    let parseAttributes(node : XElement) =
        seq {
            for attribute in node.Attributes() do
                match attribute.Name.LocalName with
                | "xmlns"
                | "x"
                | "Class" -> yield Option.None          
                | "Content"
                | "Title" -> yield addAttribute(attribute, "\"" + attribute.Value + "\"", "")            
                | "Background" 
                | "Stroke" when attribute.Value.StartsWith("#") -> 
                                                                   let extraCode = "SysColor col = System.Drawing.ColorTranslator.FromHtml(\"" + attribute.Value + "\");"
                                                                   yield addAttribute(attribute, "new SolidColorBrush(Color.FromArgb(col.A, col.R, col.G, col.B))", extraCode)
                | "Background"
                | "Stroke" -> yield addAttribute(attribute, "Brushes." + attribute.Value, "")
                | "Color" when attribute.Value.StartsWith("#") ->
                               let extraCode = "SysColor col = System.Drawing.ColorTranslator.FromHtml(\"" + attribute.Value + "\");"
                               yield addAttribute(attribute, "Color.FromArgb(col.A, col.R, col.G, col.B)", extraCode)
                | "Color" ->
                            let extraCode = "SysColor col = SysColor." + attribute.Value + ";"
                            yield addAttribute(attribute, "Color.FromArgb(col.A, col.R, col.G, col.B)", extraCode)
                | "HorizontalAlignment" -> yield addAttribute(attribute, "HorizontalAlignment." + attribute.Value, "")               
                | "VerticalAlignment" -> yield addAttribute(attribute, "VerticalAlignment." + attribute.Value, "")
                | "HorizontalScrollBarVisibility"
                | "VerticalScrollBarVisibility" -> yield addAttribute(attribute, "ScrollBarVisibility." + attribute.Value, "")
                | "Margin" -> yield addAttribute(attribute, "new Thickness(" + attribute.Value + ")", "")
                | "EndPoint"
                | "StartPoint" -> yield addAttribute(attribute, "new Point(" + attribute.Value + ")", "")
                | "Orientation" -> yield addAttribute(attribute, "Orientation." + attribute.Value, "")
                | _ -> yield addAttribute(attribute, attribute.Value, "")
        }
        |> Utils.GetValues
    member this.Get(element : XElement) = parseAttributes element