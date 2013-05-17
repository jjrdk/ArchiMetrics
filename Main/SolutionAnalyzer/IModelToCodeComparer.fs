namespace SolutionAnalyzer
open System
open System.Threading.Tasks
open Roslyn.Compilers.CSharp

type IModelToCodeComparer =
    abstract CompareAsync : codeRoot : seq<SyntaxNode> -> Task<seq<IComparisonResult>>