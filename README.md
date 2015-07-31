# ArchiMetrics

Various code analysis tools for C#.

To build simply run the build.ps1 script.

If that fails, please log an issue, because the idea is to have a one click build experience. Before logging an issue, please check that you have the latest MSBuild installed.

## About the project

ArchiMetrics is a collection of code analysis tools using Roslyn. It will calculate code metrics which can be queried using normal LINQ syntax.

The project calculates the following metrics:

### Project Level

- Cyclomatic Complexity
- LinesOfCode
- Maintainability Index
- Project Dependencies
- Type Couplings
- Abstractness
- Afferent Coupling
- Efferent Coupling
- RelationalCohesion

### Namespace Level

- Cyclomatic Complexity
- LinesOfCode
- Maintainability Index
- Project Dependencies
- Type Couplings
- Depth of Inheritance
- Abstractness

### Type Level

- Cyclomatic Complexity
- LinesOfCode
- Maintainability Index
- Project Dependencies
- Type Couplings
- Depth Of Inheritance
- Type Coupling
- Afferent Coupling
- Efferent Coupling
- Instability

### Member Level

- Cyclomatic Complexity
- Lines Of Code
- Maintainability Index
- Project Dependencies
- Type Couplings
- Number Of Parameters
- Number Of Local Variables
- Afferent Coupling
- Halstead Metrics

## Using project

If you are going to use metrics, you must install

[Microsoft Build Tools 2015 RC](http://www.microsoft.com/en-us/download/details.aspx?id=46882&WT.mc_id=rss_alldownloads_all)

You also may need to install this package (included in latest nuget package)

```
Install-Package Microsoft.Composition
```

See this sample that load solution and print cyclomatic complexity for each namespace that belong your solution

````csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using ArchiMetrics.Analysis;
using ArchiMetrics.Common;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Run();
            task.Wait();
        }

        private static async Task Run()
        {
            Console.WriteLine("Loading Solution");
            var solutionProvider = new SolutionProvider();
            var solution = await solutionProvider.Get(@"MyFullPathSolutionFile.sln");
            Console.WriteLine("Solution loaded");

            var projects = solution.Projects.ToList();

            Console.WriteLine("Loading metrics, wait it may take a while.");
            var metricsCalculator = new CodeMetricsCalculator();
            var calculateTasks = projects.Select(p => metricsCalculator.Calculate(p, solution));
            var metrics = (await Task.WhenAll(calculateTasks)).SelectMany(nm => nm);
            foreach (var metric in metrics)
                Console.WriteLine("{0} => {1}", metric.Name, metric.CyclomaticComplexity);
        }
    }
}
```
