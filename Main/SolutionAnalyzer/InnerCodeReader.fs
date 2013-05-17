namespace SolutionAnalyzer
open System
open System.Linq
open System.Text.RegularExpressions
open System.Threading.Tasks
open Roslyn.Compilers.CSharp

type internal InnerCodeReader() =
    inherit SyntaxWalker()
    static let locRegex = new Regex(@"^(?!(\s*\/\/))\s*.{3,}", RegexOptions.Compiled)
    let mutable _modelItems : ModelItem list = []
    let getProperty(prop : PropertyDeclarationSyntax) = 
        new PropertyDefinition(
                               prop.Identifier.ValueText.Trim(),
                               prop.Type.GetText().ToString().Trim(),
                               ImplementationType.OnlyInCode) :> IPropertyDefinition
    let getMethod(m : MethodDeclarationSyntax) =
        new MethodDefinition(
                                m.Identifier.ValueText.Trim(),
                                m.ReturnType.GetText().ToString().Trim(),
                                ImplementationType.OnlyInCode,
                                m.ParameterList.Parameters |> Seq.map (fun p -> (p.Type.GetText().ToString().Trim(), p.Identifier.ValueText.Trim(), ImplementationType.Unknown))) :> IMethodDefinition
    let getLinesOfCode(node : SyntaxNode) =
        node.ToFullString().Split('\n') |> Seq.sumBy(fun s -> match locRegex.IsMatch(s.Trim()) with | true -> 1 | _ -> 0)
    let createModelItem(node : TypeDeclarationSyntax, elementType : ElementType) =
        if node.Modifiers.Any(SyntaxKind.PublicKeyword) then
            let modelItem : ModelItem = new ModelItem(
                                                        node.Identifier.ValueText,
                                                        getLinesOfCode node,
                                                        elementType,
                                                        node.Members.OfType<PropertyDeclarationSyntax>() |> Seq.where(fun n -> n.Modifiers.Any(SyntaxKind.PublicKeyword)) |> Seq.map getProperty,
                                                        node.Members.OfType<MethodDeclarationSyntax>() |> Seq.where(fun n -> n.Modifiers.Any(SyntaxKind.PublicKeyword)) |> Seq.map getMethod)
            _modelItems <- modelItem :: _modelItems 
    member this.GetModel() =
        Seq.distinct _modelItems
    override this.VisitInterfaceDeclaration(node : InterfaceDeclarationSyntax) =
        createModelItem(node, ElementType.Interface)
        base.VisitInterfaceDeclaration node
    override this.VisitClassDeclaration(node : ClassDeclarationSyntax) =
        createModelItem(node, ElementType.Class)
        base.VisitClassDeclaration node