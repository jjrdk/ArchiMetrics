namespace ArchiMeter.Analysis
open System
open System.IO
open System.Threading.Tasks
open System.Text.RegularExpressions
open Roslyn.Services
open Roslyn.Compilers.Common

type SLoCCounter() =
    static let regex = new Regex(@"^(?!(\s*\/\/))\s*.{3,}", RegexOptions.Compiled)
    let countStrings(strings : seq<string>) =
        strings |> Seq.map (fun s -> s.Trim()) |> Seq.where regex.IsMatch |> Seq.length
    let countNode(node : CommonSyntaxNode) =
        let strings = node.ToFullString().Split('\n')
        countStrings strings
    let countDoc(doc : IDocument) =
        countNode(doc.GetSyntaxTree().GetRoot())
    let countDocuments(documents : seq<IDocument>) =
        documents |> Seq.distinct |> Seq.sumBy countDoc
    let isNotExcluded(directory : string, exclusions : seq<string>) =
        exclusions
        |> Seq.forall (fun e -> not (directory.ToLowerInvariant().Contains(e.ToLowerInvariant())))
    member this.Count(projects : seq<IProject>) =
        projects
        |> Seq.distinct
        |> Seq.collect (fun p -> p.Documents)
        |> Seq.sumBy countDoc
    member this.CountAsync(projects : seq<IProject>) =
        projects
        |> Seq.distinct
        |> Seq.collect (fun p -> p.Documents)
        |> Seq.map (fun d -> async { return countDoc d })
        |> Async.Parallel
        |> Async.StartAsTask
        |> (fun t -> t.ContinueWith((fun (x : Task<int[]>) -> x.Result |> Seq.sum)))
    member this.Count(solution : ISolution) =
        this.Count solution.Projects
    member this.Count(node : CommonSyntaxNode) =
        countNode node
    member this.Count(path : string, exclusions : seq<string>) =        
        let files = [|"*.cs";"*.xaml";"*.vb"|] |> Seq.collect (fun s -> Directory.GetFiles(path,s, SearchOption.AllDirectories))
        files 
        |> Seq.where (fun f -> isNotExcluded(f, exclusions))
        |> Seq.map (fun f -> async { return countStrings(File.OpenText(f).ReadToEnd().Split('\n')) }) 
        |> Async.Parallel 
        |> Async.RunSynchronously
        |> Seq.sum
    member this.Count(snippet : string) =
        countStrings(snippet.Split('\n'))