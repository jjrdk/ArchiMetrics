namespace SolutionAnalyzer
open System
open System.Collections.Generic
open System.IO
open System.Runtime.Serialization.Formatters.Binary
open System.Threading.Tasks
open System.Linq
open System.Threading.Tasks
open Roslyn.Compilers.CSharp
open ArchiMeter.Common

type ModelToCodeComparer(root : seq<EA.Package>, modelSettings : ModelSettings) =
    let compareParameters(code : IMethodDefinition, model : IMethodDefinition) = 
        let codeParameters = Set.ofSeq code.Parameters
        let modelParameters = Set.ofSeq model.Parameters
        let sharedNames = Set.intersect codeParameters modelParameters
        let codeOnlyNames = Set.difference codeParameters sharedNames
        let modelOnlyNames = Set.difference modelParameters sharedNames
        sharedNames |> Seq.map (fun (x,y,z) -> (x, y, ImplementationType.InBoth))
        |> Seq.append (codeOnlyNames |> Seq.map (fun (x,y,z) -> (x, y, ImplementationType.OnlyInCode)))
        |> Seq.append (modelOnlyNames |> Seq.map (fun (x,y,z) -> (x, y, ImplementationType.OnlyInModel)))
    let compareMethods(codeMethods : seq<IMethodDefinition>, modelMethods : seq<IMethodDefinition>) =
        let codeSet = Set.ofSeq(codeMethods)
        let modelSet = Set.ofSeq(modelMethods)
        let sharedNames = Set.intersect codeSet modelSet
        let codeOnlyNames = Set.difference codeSet sharedNames
        let modelOnlyNames = Set.difference modelSet sharedNames
        Seq.concat [|(codeOnlyNames |> Seq.map (fun (m : IMethodDefinition) -> new MethodDefinition(m.Name, m.ReturnType, ImplementationType.OnlyInCode, m.Parameters)));
                    (modelOnlyNames |> Seq.map (fun (m : IMethodDefinition) -> new MethodDefinition(m.Name, m.ReturnType, ImplementationType.OnlyInModel, m.Parameters)));
                    (sharedNames |> Seq.map (fun (m : IMethodDefinition) -> new MethodDefinition(m.Name, m.ReturnType, ImplementationType.InBoth, compareParameters(codeMethods |> Seq.find(fun n -> n.Name = m.Name), modelMethods |> Seq.find(fun n -> n.Name = m.Name)))))|]
        |> Seq.cast<IMethodDefinition>
    let compareProperties(codeProperties : seq<IPropertyDefinition>, modelProperties : seq<IPropertyDefinition>) =
        let codeSet = Set.ofSeq codeProperties
        let modelSet = Set.ofSeq modelProperties
        let sharedNames = Set.intersect codeSet modelSet
        let codeOnlyProperties = Set.difference codeSet sharedNames
        let modelOnlyProperties = Set.difference modelSet sharedNames
        Seq.concat [|codeOnlyProperties |> Seq.map (fun m -> new PropertyDefinition(m.Name, m.ReturnType, ImplementationType.OnlyInCode));
                    (modelOnlyProperties |> Seq.map (fun m -> new PropertyDefinition(m.Name, m.ReturnType, ImplementationType.OnlyInModel)));
                    (sharedNames |> Seq.map (fun m -> new PropertyDefinition(m.Name, m.ReturnType, ImplementationType.InBoth)))|]
        |> Seq.cast<IPropertyDefinition>
    let createComparisonResult(m : ModelItem, implementation : ImplementationType, m2 : option<ModelItem>) =
        let methods = match m2 with | None -> m.Methods | _ -> compareMethods(m.Methods, m2.Value.Methods)
        let properties = match m2 with | None -> m.Properties | _ -> compareProperties(m.Properties, m2.Value.Properties)
        new ComparisonResult(m.Name, m.LinesOfCode, m.Type, implementation, properties, methods)
    let compare(code : seq<ModelItem>, model : seq<ModelItem>) =
        let codeArray = Seq.toArray code;
        let modelArray = Seq.toArray model;
        let comparer : IEqualityComparer<ModelItem> = new InnerModelNameComparer() :> IEqualityComparer<ModelItem>
        let sharedItems = codeArray.Intersect(modelArray, comparer)
        let inCodeOnly = codeArray.Except(sharedItems, comparer)
        let inModelOnly = modelArray.Except(sharedItems, comparer)        
        Seq.concat [|(sharedItems |> Seq.map(fun m -> createComparisonResult(m, ImplementationType.InBoth, Some(modelArray |> Seq.where(fun x -> comparer.Equals(x, m)) |> Seq.head))));
                    (inCodeOnly |> Seq.map(fun m -> createComparisonResult(m, ImplementationType.OnlyInCode, None)));
                    (inModelOnly |> Seq.map(fun m -> createComparisonResult(m, ImplementationType.OnlyInModel, None)))|] 
        |> Seq.cast<IComparisonResult>   
    let _modelLoadTask = Async.StartAsTask(                            
                                            async {
                                                let modelSource = modelSettings.Data
                                                let formatter = new BinaryFormatter()
                                                if not (File.Exists(modelSource)) then
                                                    Console.WriteLine ("Building " + modelSettings.Name + " model")
                                                    let reader = new InnerModelReader()
                                                    (reader :> IModelVisitor).Visit(root)
                                                    let model = Seq.toArray(reader.GetModel())
                                                    try
                                                        use newFile = File.Create(modelSource)
                                                        formatter.Serialize(newFile, model) 
                                                    with
                                                    | ex -> Console.WriteLine ex.Message
                                                            Console.WriteLine ex.StackTrace
                                                    return Seq.ofArray model
                                                else
                                                    Console.WriteLine ("Using existing " + modelSettings.Name + " model")
                                                    try
                                                        let model = formatter.Deserialize(File.OpenRead(modelSource)) :?> seq<ModelItem>
                                                        return model
                                                    with
                                                    | ex -> Console.WriteLine ex.Message
                                                            Console.WriteLine ex.StackTrace
                                                            return Seq.empty
                                            })
    interface IModelToCodeComparer with
        member this.CompareAsync(codeRoot : seq<SyntaxNode>) =
            let task = Async.StartAsTask(
                                async {                                    
                                        let reader = new InnerCodeReader()
                                        codeRoot |> Seq.iter reader.Visit
                                        return reader.GetModel()                                        
                                })
            let t = [|task; _modelLoadTask|]
            Task.Factory.ContinueWhenAll(
                                         t,
                                         (fun (tasks : Task<seq<ModelItem>>[]) ->
                                                        let codeTask = tasks.[0]
                                                        let modelTask = tasks.[1]
                                                        let exMap = fun (e : Exception) -> new ComparisonResult(e.StackTrace, 0, ElementType.Action,ImplementationType.OnlyInModel,Seq.empty,Seq.empty) :> IComparisonResult
                                                        match (codeTask.Exception, modelTask.Exception) with
                                                        | (null, null) -> 
                                                                            let code = codeTask.Result
                                                                            let model = modelTask.Result
                                                                            compare(code, model)
                                                        | (a, null) -> a.InnerExceptions |> Seq.map exMap
                                                        | (null, b) -> b.InnerExceptions |> Seq.map exMap
                                                        | (a, b) -> Seq.concat [|(a.InnerExceptions |> Seq.map exMap); (b.InnerExceptions |> Seq.map exMap)|]))