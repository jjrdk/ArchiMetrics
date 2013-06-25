namespace ArchiMeter.Analysis.Xaml
open System
open System.Text.RegularExpressions
open Roslyn.Compilers.Common
open Roslyn.Compilers.CSharp
open Roslyn.Services
open Roslyn.Services.Formatting
open ArchiMeter.Common.Xaml

type internal XamlCodeWriter() =
    static let BindingRegex = new Regex("^{Binding(.+)?}$", RegexOptions.Compiled)

    let getPropertyIdentifier(prop : XamlPropertyNode) =
        match prop.Value with 
        | :? XamlNode as xn -> Syntax.IdentifierName(xn.VariableName) 
        | null -> Syntax.IdentifierName(Syntax.Token(SyntaxKind.NullKeyword)) 
        | _ as o -> Syntax.IdentifierName(o.ToString())

    let createBindingSyntax(nodeClassName : string, nodeVariableName : string, variableName : string, inlineValue : string) =
        let strings = inlineValue.Split(',') |> Seq.toArray
        let parameters = strings 
                         |> Seq.collect (fun p -> 
                                              p.Trim().Split([|','|], StringSplitOptions.RemoveEmptyEntries)
                                              |> Seq.map (fun kv -> kv.Split('='))
                                              |> Seq.map (fun t -> (t |> Seq.head, String.Join("=", t |> Seq.skip 1))))
                         |> Seq.map (fun (k,v) -> Syntax.BinaryExpression(SyntaxKind.AssignExpression, Syntax.IdentifierName(k), Syntax.IdentifierName(v.Replace("'","\""))))
        let separators = Seq.init (strings.Length - 1) (fun x -> Syntax.Token(SyntaxKind.CommaToken))
        let binding = Syntax.ObjectCreationExpression(
                                                      Syntax.ParseTypeName("Binding"), 
                                                      null, 
                                                      Syntax.InitializerExpression(
                                                                                    SyntaxKind.ObjectInitializerExpression,
                                                                                    Syntax.SeparatedList<ExpressionSyntax>(parameters |> Seq.cast<ExpressionSyntax>, separators)))
        let memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(nodeVariableName), Syntax.IdentifierName("SetBinding"))
        let dependencyProperty = Syntax.Argument(Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(nodeClassName), Syntax.IdentifierName(variableName + "Property")))
        let arguments = [|dependencyProperty; Syntax.Argument(binding)|]
        let memberInvocation = Syntax.InvocationExpression(memberAccess, Syntax.ArgumentList(Syntax.SeparatedList(arguments, [|Syntax.Token(SyntaxKind.CommaToken)|])))
        Syntax.ExpressionStatement(memberInvocation)

    let createDependencyPropertySyntax(variableName : string) (attr : XamlNodeAttribute) =
        let memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(variableName), Syntax.IdentifierName("SetValue"))
        let argList = Syntax.SeparatedList([|Syntax.Argument(Syntax.IdentifierName(attr.VariableName + "Property")); Syntax.Argument(Syntax.IdentifierName(attr.Value))|], [|Syntax.Token(SyntaxKind.CommaToken)|])
        let invocation = Syntax.InvocationExpression(memberAccess, Syntax.ArgumentList(argList))
        Syntax.ExpressionStatement(invocation)

    let createLocalDeclaration(childNode : XamlNode) = 
        Syntax.LocalDeclarationStatement(    
            Syntax.VariableDeclaration(
                Syntax.IdentifierName("var"),
                    Syntax.SeparatedList(
                        Syntax.VariableDeclarator(
                            Syntax.Identifier(childNode.VariableName),
                            null,
                            Syntax.EqualsValueClause(Syntax.ObjectCreationExpression(Syntax.ParseTypeName(childNode.ClassName), Syntax.ArgumentList(), null))))))

    let resolveKnownPropertyName(name : string) =
        match name with
        | "Resources" -> "ResourceDictionary"
        | _ -> name

    let createParentSyntax(childNode : XamlNode) =
        let parent = childNode.Parent
        match parent.ClassName with
        | "Canvas"
        | "StackPanel"
        | "DockPanel"
        | "Grid" ->
                    let childrenAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("Children"))
                    let addAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, childrenAccess, Syntax.IdentifierName("Add"))
                    Syntax.ExpressionStatement(Syntax.InvocationExpression(addAccess, Syntax.ArgumentList(Syntax.SeparatedList(Syntax.Argument(Syntax.IdentifierName(childNode.VariableName))))))
        | "Border" ->
                      let childAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("Child"))
                      Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, childAccess, Syntax.Token(SyntaxKind.EqualsToken), Syntax.IdentifierName(childNode.VariableName)))
        | "Window"
        | "UserControl" ->
                           let contentAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("Content"))
                           Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, contentAccess, Syntax.Token(SyntaxKind.EqualsToken), Syntax.IdentifierName(childNode.VariableName)))
        | "ResourceDictionary" ->
                                  let addAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("Add"))
                                  let keyAttribute = childNode.Attributes |> Seq.find (fun a -> a.VariableName = "Key")
                                  let arguments = [|Syntax.Argument(Syntax.IdentifierName(keyAttribute.Value)); Syntax.Argument(Syntax.IdentifierName(childNode.VariableName))|]
                                  let separators = [|Syntax.Token(SyntaxKind.CommaToken)|]
                                  Syntax.ExpressionStatement(Syntax.InvocationExpression(addAccess, Syntax.ArgumentList(Syntax.SeparatedList(arguments, separators))))
        | _ ->
               let variableAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(parent.VariableName), Syntax.IdentifierName("AddChild"))
               Syntax.ExpressionStatement(Syntax.InvocationExpression(variableAccess, Syntax.ArgumentList(Syntax.SeparatedList(Syntax.Argument(Syntax.IdentifierName(childNode.VariableName))))))

    let isPropertyOf(parent : XamlNode, child : XamlNode) =
        let p = parent.Properties
        p |> Seq.exists (fun n -> n.Name = child.VariableName)

    let writeProperties (propertyCreation : XamlPropertyNode -> seq<StatementSyntax>) (childNode : XamlNode) =
        seq {
            for prop in childNode.Properties do
                yield! propertyCreation prop
                let memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(childNode.VariableName), Syntax.IdentifierName(prop.Name))
                let setAccess = Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, memberAccess, Syntax.Token(SyntaxKind.EqualsToken), getPropertyIdentifier(prop)))
                yield setAccess :> StatementSyntax
        }

    let writeAttributes (childNode : XamlNode) =
        seq {
            for attr in childNode.Attributes do
                    if not (String.IsNullOrWhiteSpace attr.ExtraCode) then
                        yield Syntax.ExpressionStatement(Syntax.ParseExpression(attr.ExtraCode)) :> StatementSyntax
                    let bindingMatch = BindingRegex.Match attr.Value
                    if bindingMatch.Success then
                        let inlineValue = bindingMatch.Groups.[1].Value.Trim()
                        yield createBindingSyntax(childNode.ClassName, childNode.VariableName, attr.VariableName, inlineValue) :> StatementSyntax
                    elif attr.VariableName.Contains(".") then
                        yield createDependencyPropertySyntax childNode.VariableName attr :> StatementSyntax
                    else
                        let memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, Syntax.IdentifierName(childNode.VariableName), Syntax.IdentifierName(attr.VariableName))
                        yield Syntax.ExpressionStatement(
                                Syntax.BinaryExpression(
                                    SyntaxKind.AssignExpression, memberAccess, Syntax.Token(SyntaxKind.EqualsToken), Syntax.IdentifierName(attr.Value))) :> StatementSyntax
        }

    let rec generateChildStatement(childNode : XamlNode, propertyCreation : XamlPropertyNode -> seq<StatementSyntax>) =
        seq{
            yield createLocalDeclaration childNode :> StatementSyntax
            yield! writeProperties propertyCreation childNode
            yield! writeAttributes childNode
            if childNode.Parent <> null && not (isPropertyOf(childNode.Parent, childNode)) then 
                yield createParentSyntax(childNode) :> StatementSyntax
            yield! childNode.Children |> Seq.collect (fun c -> generateChildStatement(c, propertyCreation))
        }

    let createPropertyAssignment(prop : XamlPropertyNode, ownerExpression : ExpressionSyntax) = 
        match prop.Value with
                | :? seq<XamlNode> as values -> values |> Seq.map (fun v ->
                                                                            let memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, ownerExpression, Syntax.IdentifierName(prop.Name))
                                                                            let addAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, memberAccess, Syntax.IdentifierName("Add"))
                                                                            Syntax.ExpressionStatement(Syntax.InvocationExpression(addAccess, Syntax.ArgumentList(Syntax.SeparatedList(Syntax.Argument(Syntax.IdentifierName(v.VariableName)))))) :> StatementSyntax)
                                                        |> Seq.toArray
                                                        |> Seq.ofArray
                | _ -> seq
                          {
                            let memberAccess = Syntax.MemberAccessExpression(SyntaxKind.MemberAccessExpression, ownerExpression, Syntax.IdentifierName(prop.Name))
                            yield Syntax.ExpressionStatement(Syntax.BinaryExpression(SyntaxKind.AssignExpression, memberAccess, Syntax.Token(SyntaxKind.EqualsToken), getPropertyIdentifier prop)) :> StatementSyntax
                          }

    let rec createPropertySyntax(property : XamlPropertyNode, ownerSyntax : ExpressionSyntax) =
        match property.Value with
        | :? XamlNode as node -> generateChildStatement(node, (fun p -> createPropertySyntax(p, ownerSyntax)))
        | :? seq<XamlNode> as nodes -> nodes |> Seq.collect (fun n -> generateChildStatement(n, (fun p -> createPropertySyntax(p, ownerSyntax))))
        | _ -> Seq.empty

    let createSyntax(nodes : seq<XamlNode>, ownerSyntax : ExpressionSyntax) =
        nodes 
        |> Seq.collect (fun n -> generateChildStatement(n, fun p -> createPropertySyntax(p, ownerSyntax)))
        |> Seq.map (fun s -> SyntaxExtensions.WithTrailingTrivia(s, Syntax.ElasticCarriageReturnLineFeed))

    member this.CreateSyntax(node : XamlNode) =
        let properties = node.Properties |> Seq.collect (fun p -> createPropertySyntax(p, Syntax.ThisExpression()))
        let assignments = node.Properties |> Seq.collect (fun p -> createPropertyAssignment(p, Syntax.ThisExpression()))

        let codeRoot = Seq.concat [|properties; assignments; createSyntax (node.Children, Syntax.IdentifierName(node.VariableName))|]
        let classDeclarationSyntax = match String.IsNullOrWhiteSpace(node.BaseClassName) with 
                                     | false -> Syntax.ClassDeclaration(node.ClassName)
                                                      .AddModifiers(Syntax.Token(SyntaxKind.PublicKeyword))
                                                      .AddModifiers(Syntax.Token(SyntaxKind.PartialKeyword))
                                     | _ ->  Syntax.ClassDeclaration(node.ClassName)
                                                   .AddModifiers(Syntax.Token(SyntaxKind.PublicKeyword))
                                                   .AddModifiers(Syntax.Token(SyntaxKind.PartialKeyword))
                                                   .WithBaseList(Syntax.BaseList(
                                                                            Syntax.SeparatedList(
                                                                                Syntax.ParseTypeName(node.BaseClassName))))
        let xamlClass = classDeclarationSyntax
                            .WithMembers(
                                Syntax.List<MemberDeclarationSyntax>(
                                    Syntax.MethodDeclaration(
                                        Syntax.PredefinedType(
                                            Syntax.Token(SyntaxKind.VoidKeyword)),
                                        "InitializeComponent")
                                        .WithModifiers(
                                            Syntax.TokenList(Syntax.Token(SyntaxKind.ProtectedKeyword)))
                                        .WithBody(Syntax.Block(codeRoot))))
        let usings = Syntax.List(node.Usings |> Seq.map (fun kvp -> if String.IsNullOrWhiteSpace(kvp.Value) then
                                                                        Syntax.UsingDirective(Syntax.ParseName(kvp.Key)) 
                                                                       else 
                                                                        Syntax.UsingDirective(Syntax.NameEquals(Syntax.IdentifierName(kvp.Value)), Syntax.ParseName(kvp.Key))))
        let namespaceName = match String.IsNullOrWhiteSpace(node.NamespaceName) with | true -> "ArchiMeter" | _ -> node.NamespaceName
        let namespaceSyntax = Syntax.NamespaceDeclaration(Syntax.ParseName(namespaceName)).WithUsings(usings).AddMembers(xamlClass).Format(FormattingOptions.GetDefaultOptions()).GetFormattedRoot() :?> MemberDeclarationSyntax
        let compilationUnit = Syntax.CompilationUnit().AddMembers(namespaceSyntax)
        SyntaxTree.Create(compilationUnit)