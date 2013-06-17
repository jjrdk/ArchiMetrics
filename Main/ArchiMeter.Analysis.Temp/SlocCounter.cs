namespace ArchiMeter.Analysis
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using ArchiMeter.Common;
	using Roslyn.Compilers.Common;
	using Roslyn.Services;
	using Roslyn.Services.Formatting;

	public class SlocCounter
	{
		private static readonly Regex LinePattern = new Regex(@"^(?!(\s*\/\/))\s*.{3,}", RegexOptions.Compiled);
		private static readonly string[] Extensions = new[] { "*.cs", "*.xaml", "*.vb" };

		public int Count(string snippet)
		{
			return CountStrings(snippet.Split('\n'));
		}

		public int Count(string path, IEnumerable<string> exclusions)
		{
			return Extensions
				.SelectMany(s => Directory.GetFiles(path, s, SearchOption.AllDirectories))
				.AsParallel()
				.Where(s => !exclusions.Any(e => e.ToLowerInvariant().Contains(s.ToLowerInvariant())))
				.Select(s => File.OpenText(s).ReadToEnd().Split('\n'))
				.Select(CountStrings)
				.Sum();
		}

		public int Count(IEnumerable<IProject> projects)
		{
			return projects
				.Distinct(ProjectComparer.Default)
				.SelectMany(p => p.Documents)
				.Sum(d => CountDoc(d));
		}

		public int Count(ISolution solution)
		{
			return Count(solution.Projects);
		}

		private int CountDoc(IDocument document)
		{
			return this.CountNode(document.GetSyntaxTree().GetRoot());
		}

		private int CountNode(CommonSyntaxNode node)
		{
			var lines = node.Format(FormattingOptions.GetDefaultOptions()).GetFormattedRoot().ToFullString().Split('\n');
			return CountStrings(lines);
		}

		private static int CountStrings(IEnumerable<string> strings)
		{
			return strings.Select(s => s.Trim()).Count(s => LinePattern.IsMatch(s));
		}

		/*
		 type SLoCCounter() =
   
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
		 */
	}
}