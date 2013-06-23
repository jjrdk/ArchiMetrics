using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ArchiCop.VisualStudioData
{
    public class VisualStudioSolutionRepository : IVisualStudioSolutionRepository
    {
        public void CreateNewSolution(VisualStudioProjectGraph graph, string solutionFileName)
        {
            Assembly assembly = GetType().Assembly;
            var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("ArchiCop.Core.Template_sln"));

            string sol = textStreamReader.ReadToEnd();

            var projectIncludes = new StringBuilder();

            /*
            Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ArchiCopCore", "ArchiCopCore\ArchiCopCore.csproj", "{3A4D7180-400E-486E-84B9-ADF89DDB790F}"
            EndProject
            */

            string projectTypeGuid = "";

            foreach (VisualStudioProject project in graph.Vertices)
            {
                switch (project.ProjectType)
                {
                    case ".csproj":
                        projectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
                        break;
                    case ".fsproj":
                        projectTypeGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}";
                        break;
                }

                projectIncludes.AppendLine(
                    string.Format(
                        "Project(\"{0}\") = \"{1}\", \"{2}\", \"{{{3}}}\"",
                        projectTypeGuid, project.ProjectName, MakeRelativePath(project.ProjectPath, solutionFileName),
                        project.ProjectGuid));
                projectIncludes.AppendLine("EndProject");
            }

            sol = sol.Replace("{projects}", projectIncludes.ToString());

            using (var file = new StreamWriter(new FileStream(solutionFileName, FileMode.Create)))
            {
                file.Write(sol);
            }
        }

        private static string MakeRelativePath(string fromPath, string relativeTo)
        {
            if (string.IsNullOrWhiteSpace(fromPath))
            {
                throw new ArgumentException("fromPath");
            }
            if (string.IsNullOrWhiteSpace(relativeTo))
            {
                throw new ArgumentException("relativeTo");
            }

            var fromUri = new Uri(Path.GetFullPath(fromPath));
            var toUri = new Uri(Path.GetFullPath(relativeTo));

            Uri relativeUri = toUri.MakeRelativeUri(fromUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}